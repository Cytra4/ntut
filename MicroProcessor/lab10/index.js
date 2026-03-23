const { spawn, exec } = require('child_process');
const express = require('express');
const path = require('path');
const app = express();

let sharedData = { adc: 0, freq: 0, isWorking: false, lastResult: null };

// Thread A: 持續監控亮度
const monitor = spawn('python3', ['-u', 'controll.py']);
monitor.stdout.on('data', (data) => {
    const line = data.toString().trim();
    if (line.startsWith("ADC=")) {
        const parts = line.split(',');
        sharedData.adc = parseInt(parts[0].split('=')[1]);
    }
});

app.get('/', (req, res) => res.sendFile(path.join(__dirname, 'index.html')));
app.get('/api/status', (req, res) => res.json(sharedData));

// Thread B & C 互動邏輯
app.post('/api/start', (req, res) => {
    if (sharedData.adc > 300) { 
        return res.json({ success: false, message: `環境太暗 (${sharedData.adc})，拒絕運算！` });
    }

    sharedData.isWorking = true;
    sharedData.lastResult = "運算中..."; // 先給一個預設值，讓網頁知道開始動了
    
    console.log(">>> 啟動運算任務...");
    exec('python3 led_control.py 4 ON');

    const worker = spawn('python3', ['-u', 'worker_task.py']);
    let workerOutput = "";

    worker.stdout.on('data', (data) => {
        workerOutput += data.toString();
    });

    worker.on('close', (code) => {
        exec('python3 led_control.py 4 OFF');
        sharedData.isWorking = false;
        
        // 重要：確保存入的是字串且去掉多餘空格
        sharedData.lastResult = workerOutput.trim(); 
        
        console.log(">>> 後端運算完成，結果已存入 sharedData");
    });

    res.json({ success: true, message: "運算已啟動" });
});
app.listen(3000, () => console.log("Server running at http://localhost:3000"));
