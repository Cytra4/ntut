const express = require('express');
const cors = require('cors');
const { execFile } = require('child_process');
const path = require('path');

const app = express();
app.use(cors());
app.use(express.json());
// Serve static frontend files from project root
const staticRoot = path.join(__dirname);
app.use(express.static(staticRoot));

// In-memory device state
// device state; includes lastIntrusionAt to prevent immediate override by auto LDR
const devices = [
  { id: 'door1', name: '門 1', type: '門', detection: false, status: '安全', lastIntrusionAt: null },
  { id: 'door2', name: '門 2', type: '門', detection: false, status: '安全', lastIntrusionAt: null },
  { id: 'win1', name: '窗 1', type: '窗', detection: false, status: '安全', lastIntrusionAt: null },
  { id: 'win2', name: '窗 2', type: '窗', detection: false, status: '安全', lastIntrusionAt: null }
];

// how long to ignore auto-LDR driven state changes after an intrusion (ms)
const INTRUSION_HOLD_MS = 10000; // default 10 seconds

let blinkInterval = 500;

function callControlScript(args, cb) {
  // call the python script in the same folder; it's a placeholder for actual GPIO control
  const script = path.join(__dirname, 'control_gpio.py');
  execFile('python', [script, ...args], (err, stdout, stderr) => {
    if (err) console.error('control script error:', err, stderr);
    else console.log('control script:', stdout.trim());
    if (cb) cb(err, stdout, stderr);
  });
}

app.get('/api/devices', (req, res) => {
  res.json({ devices, blinkInterval });
});

// fallback for root to serve index.html (static middleware already handles it)
app.get('/', (req, res) => {
  res.sendFile(path.join(staticRoot, 'index.html'));
});

app.post('/api/device/:id/detection', (req, res) => {
  const id = req.params.id;
  const { detection } = req.body;
  const d = devices.find(x => x.id === id);
  if (!d) return res.status(404).json({ error: 'not found' });
  d.detection = !!detection;
  if (!d.detection) d.status = '未偵測';
  else if (d.status !== '出現闖入者') d.status = '安全';
  // call control script to set LED / detection hardware
  callControlScript(['set_detection', id, d.detection ? 'on' : 'off']);
  res.json({ ok: true, device: d });
});

app.post('/api/intrude/:id', (req, res) => {
  const id = req.params.id;
  const d = devices.find(x => x.id === id);
  if (!d) return res.status(404).json({ error: 'not found' });
  if (d.detection) {
    d.status = '出現闖入者';
    d.lastIntrusionAt = Date.now();
    callControlScript(['trigger_alarm', id, String(blinkInterval)]);
    res.json({ alarm: true, device: d });
  } else {
    d.status = '闖入成功';
    res.json({ alarm: false, device: d });
  }
});

app.post('/api/reset', (req, res) => {
  devices.forEach(d => {
    if (d.status === '出現闖入者') {
      d.status = '安全';
      d.detection = true; // keep detection enabled after reset
      d.lastIntrusionAt = null;
      callControlScript(['stop_alarm', d.id]);
    }
  });
  res.json({ ok: true, devices });
});

app.post('/api/blink', (req, res) => {
  const { interval } = req.body;
  const i = Math.max(50, parseInt(interval, 10) || 500);
  blinkInterval = i;
  res.json({ ok: true, blinkInterval });
});

app.post('/api/ldr', (req, res) => {
  const { value, threshold } = req.body;
  const v = parseInt(value, 10) || 0;
  const t = parseInt(threshold, 10) || 0;
  const shouldOn = v >= t;
  devices.forEach(d => {
    // If device has a recent intrusion, keep its alert status for a hold period
    const now = Date.now();
    if (d.status === '出現闖入者' && d.lastIntrusionAt && (now - d.lastIntrusionAt) < INTRUSION_HOLD_MS) {
      // skip auto-update for this device during hold
      return;
    }
    d.detection = shouldOn;
    if (!shouldOn) d.status = '未偵測';
    else if (d.status !== '出現闖入者') d.status = '安全';
    callControlScript(['set_detection', d.id, d.detection ? 'on' : 'off']);
  });
  res.json({ ok: true, devices });
});

app.get('/api/ldr/read', (req, res) => {
  // call control script to read LDR and return numeric value
  const script = path.join(__dirname, 'control_gpio.py');
  execFile('python', [script, 'read_ldr'], (err, stdout, stderr) => {
    if (err) {
      console.error('ldr read error', err, stderr);
      return res.json({ ok: false, value: null });
    }
    const out = stdout.trim();
    const val = parseInt(out.split('\n').pop(), 10);
    if (Number.isNaN(val)) return res.json({ ok: false, value: null, raw: out });
    // Optionally apply threshold logic here if desired
    res.json({ ok: true, value: val, raw: out });
  });
});

const port = process.env.PORT || 3000;
app.listen(port, () => console.log(`API server listening on http://localhost:${port}`));
