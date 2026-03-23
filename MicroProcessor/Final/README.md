# Smart Alarm + LED Simulator

簡介：此專案包含前端模擬介面 (`index.html`) 與簡單的 Node.js 後端 (`server.js`)。前端會對後端發送請求；後端可再呼叫 `control_gpio.py` 或其他程式以實際控制 TX2 上的 GPIO。

快速啟動（在專案目錄）:

```bash
npm install
npm start
```

預設伺服器監聽 `http://localhost:3000`，開啟瀏覽器並載入 `index.html` 即可操作前端模擬介面（注意：如果直接從檔案系統開啟 `index.html`，請確保前端與後端在同一網域或透過 HTTP 服務 static 提供）。

控制脚本：`control_gpio.py` 是範例 placeholder，會印出要做的動作。要在 Jetson TX2 上實際控制 GPIO，可使用 `Jetson.GPIO` 或其他適用庫，並把相關程式碼填入 `control_gpio.py` 中。

安全與部署提示：在實機環境請確認 Node.js 與 Python 權限、GPIO 存取權限，以及將敏感控制 API 加上認證與保護。
