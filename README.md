# REAL.NET

[![Join the chat at https://gitter.im/REAL-NET/Lobby](https://badges.gitter.im/REAL-NET/Lobby.svg)](https://gitter.im/REAL-NET/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

 Travis        | AppVeyor
 ------------- | --------------
[![Travis Build Status](https://travis-ci.org/yurii-litvinov/REAL.NET.svg?branch=master)](https://travis-ci.org/yurii-litvinov/REAL.NET) | [![AppVeyor Build status](https://ci.appveyor.com/api/projects/status/2midbuo5dlq6vt8d?svg=true)](https://ci.appveyor.com/project/yurii-litvinov/real-net)

[![BCH compliance](https://bettercodehub.com/edge/badge/yurii-litvinov/REAL.NET?branch=master)](https://bettercodehub.com/)

A set of .NET libraries for quick creation of visual languages and related tools. Spiritual successor of QReal project (https://github.com/qreal/qreal)

## Usage

As a project under heavy development it has no practical usages yet. Planned features are related to ability of quickly and declaratively specify 
a visual language and get an editor, code generator and other tools for it ready to use in any .NET program.

See [API Reference](https://yurii-litvinov.github.io/REAL.NET/reference) page for documentation on what we have so far.

## Build

* Make sure you have .NET Core 2.0 or newer and F# SDK installed
* To build it from console, run `build.sh` or `build.cmd`
* To build it from Visual Studio, you will need VisualStudio 2017 and Paket.VisualStudio extension 
  (it is easily obtainable from inside Visual Studio, Extensions and Updates menu item).

## AirSim

First visual domain specific language made on REAL.NET is language for simulated by AirSim drones.
You can download stand-alone app using [this link](https://drive.google.com/open?id=1FrQxmErz7r0Q8nWjjAsuYUslZ_PtQwtW) and test it.
