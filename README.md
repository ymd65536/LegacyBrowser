
# IEみたいなブラウザアプリケーション

## 開発環境

- dotnet --version
  - 5.0.202
- VSCode
  - 1.63.0 x64

## コンパイル方法

```
 dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained true
```

## 他のアプリケーションとの連携

VBAと連携可能

## 注意点

WebBrowser コントロールを利用している為、WebBrowserコントロールのサポートがないと動作しない。  
index.htmlはCドライブに配置したモノを参照している為、事前に置いておく必要がある。
