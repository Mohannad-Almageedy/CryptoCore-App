# CryptoEdu — Educational Cryptography Suite

<div align="center">

![.NET 8](https://img.shields.io/badge/.NET-8.0-purple?logo=dotnet)
![Windows Forms](https://img.shields.io/badge/UI-Windows%20Forms-blue)
![MaterialSkin](https://img.shields.io/badge/Theme-MaterialSkin.2-deepPurple)
![SOLID](https://img.shields.io/badge/Architecture-SOLID-green)
![License](https://img.shields.io/badge/License-MIT-yellow)

**A professional, educational desktop application for cryptography built with C# and Windows Forms (.NET 8).**

</div>

---

## ✨ Features

### 📚 Classical Ciphers (Educational & Step-by-Step)
Each classical cipher provides:
- ✅ Full **Encrypt** / **Decrypt** operations
- ✅ **Mathematical Rule** display (formulas like `C = (P + K) mod 26`)
- ✅ **Step-by-Step Breakdown** — see every character substitution or matrix operation in real time

| Cipher | Type | Key |
|---|---|---|
| Caesar Cipher | Substitution | Integer shift value |
| Monoalphabetic Cipher | Substitution | 26-character alphabet |
| Playfair Cipher | Substitution | Keyword (5x5 matrix) |
| Hill Cipher | Substitution | 2x2 or 3x3 matrix |
| Vigenère Cipher | Polyalphabetic | Keyword (repeating) |
| One-Time Pad | Polyalphabetic | Key ≥ plaintext length |
| Rail Fence | Transposition | Number of rails |
| Row/Column Transposition | Transposition | Keyword |

### 🔒 Modern Encryption
| Algorithm | Type | Security |
|---|---|---|
| AES-256 (CBC) | Symmetric | Military Grade |
| Triple DES (3DES) | Symmetric | Legacy (educational) |
| RSA 2048-bit | Asymmetric | Industry Standard |

### 🔑 Hashing
- MD5, SHA-1, SHA-256, SHA-512
- HMAC-SHA256 with custom secret key

### 📁 File Encryption
- Encrypt **any file** (images, PDFs, videos) using AES-256 streaming
- Progress bar for large files — no RAM overload

### 🛠️ More Tools
- **Key / Password Generator** — cryptographically secure random keys
- **RSA Key Pair Generator** — generate + export 2048-bit public/private keys  
- **Operation History Log** — tracks all your encryption sessions

---

## 🏗️ Architecture

This project applies **SOLID principles** and professional software design patterns:

```
CryptoEdu/
├── Core/
│   ├── Interfaces/          # IClassicalCipher, ICipher (Strategy Pattern)
│   ├── Models/              # CipherStep model (Step-by-Step tracing)
│   ├── Classical/           # 8 Educational Cipher Implementations
│   ├── Modern/              # AES, 3DES, RSA Professional Engines
│   └── CipherRegistry.cs   # Central registry (DI-ready)
├── Services/                # FileEncryption, Hashing, Keys, History, Clipboard
└── UI/
    ├── Forms/               # MainForm application shell
    └── UserControls/        # Reusable &#8203;ClassicalCipherPanel, CipherDetailControl...
```

### Design Patterns Used
- **Strategy Pattern**: `IClassicalCipher` and `ICipher` allow easy algorithm swapping
- **Registry Pattern**: `CipherRegistry` provides centralized algorithm access
- **SRP (Single Responsibility)**: Each class has one clear purpose
- **OCP (Open/Closed)**: Adding a new cipher requires zero changes to existing code

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows OS (for Windows Forms)

### Build & Run
```bash
git clone https://github.com/Mohannad-Almageedy/CryptoCore-App.git
cd CryptoCore-App
dotnet build
dotnet run
```

---

## 📖 Educational Use

CryptoEdu was built to help university students visualize how classical ciphers work internally. The step-by-step breakdown feature shows every substitution, matrix computation, or transposition as it happens.

---

## 👨‍💻 Author

**Mohannad Almageedy**  
Portfolio project demonstrating: C#, Windows Forms, Cryptography, SOLID Principles, Clean Code

---
