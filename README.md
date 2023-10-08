此分支对应目前仍在微软商店的旧版应用[网络唤醒（Wake on LAN）](https://apps.microsoft.com/detail/%25E7%25BD%2591%25E7%25BB%259C%25E5%2594%25A4%25E9%2586%2592%25EF%25BC%2588wake-on-lan%25EF%25BC%2589/9N0JJ4VHZ6X5?hl=zh-cn&gl=CN)，该分支所对应的实现已被废弃，目前已停止一切非必要的开发动作。

---

<p align="center">
  <h1 align="center">网络唤醒（Wake on LAN）</h1>
  <p align="center">一个基于 WinUI3 的网络唤醒 (Wake on LAN) 客户端。</p>
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

[<img src="https://get.microsoft.com/images/zh-cn%20light.svg"  width="30%" height="30%">](https://apps.microsoft.com/store/detail/%E7%BD%91%E7%BB%9C%E5%94%A4%E9%86%92%EF%BC%88wake-on-lan%EF%BC%89/9N0JJ4VHZ6X5)

### 从 Releases 获取自签名版（不推荐）

您可以直接到 [Releases · Direct5dom/WinWoL](https://github.com/Direct5dom/WinWoL/releases) 下载我已经打包好的安装包。

> 需要注意的是，因为本项目使用自签名旁加载，需要您打开Windows的开发者模式，右键“使用PowrShell”运行`Install.ps1`，而不是直接双击`WinWoL.msix`。

> 自签名版需要打开开发者模式并安装一个证书，这并不是安全的应用安装方式。

<!-- ## 使用教程/Wiki

参考本项目[Wiki](https://github.com/Direct5dom/WinWoL/wiki)。 -->

## 🛠️获取源码/Source Code

要构建此项目，您需要将项目源码克隆到本地。

您可以使用 Git 命令行：

```powershell
git clone https://github.com/Direct5dom/WinWoL.git
```

或者更方便的，使用 Visual Studio 的“克隆存储库”克隆本仓库。

使用 Visual Studio 打开项目根目录的 `WinWoL.sln`，即可进行调试和打包。