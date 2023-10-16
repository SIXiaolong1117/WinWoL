<p align="center">
  <h1 align="center">远程工具箱 (Remote Toolbox)</h1>
  <p align="center">一个基于 WinUI3 的远程工具箱 (Remote Toolbox)。</p>
  <p align="center">
    <a href="https://github.com/Direct5dom/WinWoL/blob/master/LICENSE">
      <img src="https://img.shields.io/github/license/Direct5dom/WinWoL"/>
    </a>
    <a href="https://github.com/Direct5dom/WinWoL/releases">
      <img src="https://img.shields.io/github/v/release/Direct5dom/WinWoL?display_name=tag"/>
    </a>
  </p>
  <p align="center">
    <a href="https://twitter.com/SI_Xiaolong">
      <img src="https://img.shields.io/badge/follow-SI_Xiaolong-blue?style=flat&logo=Twitter"/>
    </a>
  </p>
</p>
<p align="center">
    <img src="./README/1.png" width="49%"/>
    <img src="./README/2.png" width="49%"/>
</p>

## ⬇下载/Download

### 从 Microsoft Store 获取（推荐）

[<img src="https://get.microsoft.com/images/zh-cn%20light.svg"  width="30%" height="30%">](https://www.microsoft.com/store/apps/9N0JJ4VHZ6X5)

### 从 Releases 获取自签名版（不推荐）

您可以直接到 [Releases · Direct5dom/WinWoL](https://github.com/Direct5dom/WinWoL/releases) 下载我已经打包好的安装包。

> 需要注意的是，因为本项目使用自签名旁加载，需要您打开Windows的开发者模式，右键“使用PowrShell”运行`Install.ps1`，而不是直接双击`WinWoL.msix`。

> 自签名版需要打开开发者模式并安装一个证书，这并不是安全的应用安装方式。

## ✋使用教程/Wiki

参考本项目[Wiki](https://github.com/Direct5dom/WinWoL/wiki)。

## 🛠️获取源码/Source Code

要构建此项目，您需要将项目源码克隆到本地。

您可以使用 Git 命令行：

```powershell
git clone https://github.com/Direct5dom/WinWoL.git
```

或者更方便的，使用 Visual Studio 的“克隆存储库”克隆本仓库。

使用 Visual Studio 打开项目根目录的 `WinWoL.sln`，即可进行调试和打包。