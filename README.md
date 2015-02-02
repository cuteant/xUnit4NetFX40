xUnit 官方网址：http://xunit.github.io/

由于xUnit v2 官方只支持.net4.5，所以修改源码使xUnit V2支持.net4.0的项目

首先设置NuGet, 加入源：

xUnit.net Nightly Builds（http://www.myget.org/F/xunit）
xUnit Dev Packages （http://www.myget.org/F/b4ff5f68eccf4f6bbfed74f055f88d8f/）

已获取最新的xUnit安装包和开发包

项目文件配置参考Sample\TestOrderExamples.Net40.csproj

项目文件开始处加入：
  <Import Project="xunit\xunit.runner.visualstudio.props" />
  <Import Project="xunit\Net40\xunit.core.props" />
  
.Net4.0的项目，在结尾处加入：
  <Import Project="xunit\Microsoft.Bcl.Build\build\Microsoft.Bcl.Build.targets" />

项目所用组件：

Microsoft.Bcl.1.1.9
Microsoft.Bcl.Async.1.0.168
Microsoft.Bcl.Build.1.0.21
Microsoft.VisualStudio.TestPlatform.ObjectModel.0.0.4
xunit.abstractions.2.0.0-rc1-build2839
xunit.runner.utility.2.0.0-rc1-build2839

使用NuGet获取最新版，或从项目DotNetFXExtension(https://git.oschina.net/cuteant/DotNetFXExtension.git)中获取

