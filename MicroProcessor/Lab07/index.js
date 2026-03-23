// index.js
// Express server + socket.io + child_process 控制 Python (controll.py)
const express = require('express');
const http = require('http');
const { spawn } = require('child_process');
const readline = require('readline');
const path = require('path');

const app = express();
const server = http.createServer(app);
const io = require('socket.io')(server);

// static files
app.use(express.json());
app.use(express.static(path.join(__dirname, 'public')));

let pythonProc = null;
let rl = null;

// route: 前端送來切換模式
app.post('/set_mode', (req, res) => {
  const mode = req.body.mode;
  console.log('Received mode:', mode);

  if (mode === 'DETECT') {
    // 若已在偵測就回傳
    if (pythonProc) {
      return res.json({ status: 'ok', message: 'already running' });
    }
    // spawn controll.py（假設檔名為 controll.py）
    pythonProc = spawn('python3', ['controll.py'], { cwd: __dirname });

    // 逐行讀 stdout（解析 ADC=...,FREQ=...）
    rl = readline.createInterface({ input: pythonProc.stdout });

    rl.on('line', (line) => {
      // 範例： "ADC=123,FREQ=2.345" 或其他印出
      // 先把整行印在 server console
      console.log('PY:', line);

      const m = line.match(/ADC=(\d+),FREQ=([\d.]+)/);
      if (m) {
        const adc = parseInt(m[1], 10);
        const freq = parseFloat(m[2]);
        // 將資料透過 socket.io 廣播給連線的前端
        io.emit('adc_update', { adc, freq });
      } else {
        // 若不是資料行，可視情況 broadcast 原始訊息
        // io.emit('py_msg', { text: line });
      }
    });

    pythonProc.stderr.on('data', (data) => {
      console.error('PY ERR:', data.toString());
      io.emit('py_error', { text: data.toString() });
    });

    pythonProc.on('close', (code, signal) => {
      console.log(`Python exited. code=${code} signal=${signal}`);
      pythonProc = null;
      if (rl) {
        try { rl.close(); } catch (e) {}
        rl = null;
      }
      io.emit('detector_stopped', { code, signal });
    });

    return res.json({ status: 'ok', message: 'detector started' });
  }

  // NO_DETECT: 停止 Python（若有在跑）
  if (mode === 'NO_DETECT') {
    if (pythonProc) {
      // 優雅關閉
      pythonProc.kill('SIGINT'); // controll.py 有處理 SIGINT
      pythonProc = null;
      if (rl) {
        try { rl.close(); } catch (e) {}
        rl = null;
      }
      return res.json({ status: 'ok', message: 'detector stopping' });
    } else {
      return res.json({ status: 'ok', message: 'not running' });
    }
  }

  res.json({ status: 'error', message: 'unknown mode' });
});

// socket.io 連線事件（可用來 log）
io.on('connection', (socket) => {
  console.log('Client connected', socket.id);
  socket.on('disconnect', () => {
    console.log('Client disconnected', socket.id);
  });
});


// ---- LED manual control API ----

// 請根據實際接線修改這兩個 BCM 編號 (這是 Python 端要操作的 BCM)
const LED1_PIN = 4;   // 對應你的 output_pin1
const LED2_PIN = 17;  // 對應你的 output_pin2

// POST /led
// JSON body: { led: "LED1" | "LED2", action: "ON" | "OFF" }
app.post('/led', express.json(), async (req, res) => {
  try {
    const body = req.body || {};
    const led = body.led;
    const action = (body.action || '').toUpperCase();

    if (!['LED1', 'LED2'].includes(led) || !['ON', 'OFF'].includes(action)) {
      return res.status(400).json({ ok: false, msg: 'invalid params' });
    }

    const pin = (led === 'LED1') ? LED1_PIN : LED2_PIN;
    // spawn Python script to set pin
    // led_control.py <pin> <ON|OFF>
    const py = spawn('python3', ['led_control.py', String(pin), action], { cwd: __dirname });

    let stdout = '';
    let stderr = '';
    py.stdout.on('data', (d) => { stdout += d.toString(); });
    py.stderr.on('data', (d) => { stderr += d.toString(); });

    py.on('close', (code) => {
      if (code === 0) {
        // success
        return res.json({ ok: true, led, action, msg: stdout.trim() });
      } else {
        console.error('led_control.py error', code, stderr);
        return res.status(500).json({ ok: false, led, action, err: stderr || 'python error' });
      }
    });
  } catch (err) {
    console.error('led route err', err);
    res.status(500).json({ ok: false, err: String(err) });
  }
});

// 啟動 server
const PORT = process.env.PORT || 3000;
server.listen(PORT, () => {
  console.log(`Server listening on http://localhost:${PORT}`);
});
