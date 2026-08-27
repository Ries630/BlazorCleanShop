# BlazorCleanShop

[![CI](https://github.com/Ries630/BlazorCleanShop/actions/workflows/ci.yml/badge.svg)](https://github.com/Ries630/BlazorCleanShop/actions/workflows/ci.yml)

BlazorCleanShop は、クリーンアーキテクチャ、軽量 DDD、Minimal APIs、TDD を題材にした
学習用のショッピングサイトです。Blazor UI と Minimal API を一つの ASP.NET Core ホストで
実行します。

## 技術スタック

- .NET 10 / C# 14 / ASP.NET Core Blazor Web App（Interactive Server）
- Minimal APIs / Microsoft.AspNetCore.OpenApi / Scalar
- Entity Framework Core 10 / SQLite / ASP.NET Core Identity
- Tailwind CSS 4 / daisyUI 5 / QuickGrid
- xUnit v3 / Microsoft Testing Platform v2 / Moq

## 前提環境

| ツール | バージョン・用途 |
|---|---|
| [.NET SDK](https://dotnet.microsoft.com/download/dotnet/10.0) | 10.0.x |
| [Node.js](https://nodejs.org/) | 24.x（npm を含む） |
| Git | リポジトリの取得に使用 |

SQLite は NuGet パッケージ経由で使用するため、SQLite CLI の別途インストールは不要です。
Identity 用 DB はリポジトリに含めず、以下の手順でローカルに生成します。

## セットアップ

```bash
git clone https://github.com/Ries630/BlazorCleanShop.git
cd BlazorCleanShop

npm ci
dotnet restore BlazorCleanShop.sln
dotnet tool restore
dotnet ef database update \
  --project src/BlazorCleanShop.Web/BlazorCleanShop.Web.csproj
```

HTTPS 開発証明書をまだ信頼していない場合は、次のコマンドを実行します。

```bash
dotnet dev-certs https --trust
```

## 開発サーバーの起動

CSS の変更を監視するプロセスと Web ホストを別々のターミナルで起動します。

```bash
npm run css:watch
```

```bash
dotnet run \
  --project src/BlazorCleanShop.Web/BlazorCleanShop.Web.csproj \
  --launch-profile https
```

- Web UI: <https://blazorcleanshop.dev.localhost:7262>
- Scalar API Reference: <https://blazorcleanshop.dev.localhost:7262/scalar/v1>

## ビルドとテスト

CI と同じ主要な検証は次のコマンドで実行できます。

```bash
npm run css:build
dotnet restore BlazorCleanShop.sln
dotnet build BlazorCleanShop.sln \
  --configuration Release \
  --no-restore \
  -m:1 \
  -nr:false
dotnet test \
  --project tests/BlazorCleanShop.Tests/BlazorCleanShop.Tests.csproj \
  --configuration Release \
  --no-build \
  --no-restore
```

Pull Request と `main` への push では、[GitHub Actions](https://github.com/Ries630/BlazorCleanShop/actions/workflows/ci.yml) が
CSS 生成、.NET のビルド、MTP v2 テストを実行します。

## CSS の編集

- 編集元は `src/BlazorCleanShop.Web/Styles/tailwind.css` と Razor コンポーネントです。
- Tailwind CSS のユーティリティは `tw:`、daisyUI のコンポーネントは `tw:d-` 接頭辞を使います。
  例: `tw:p-4`、`tw:d-btn`
- `src/BlazorCleanShop.Web/wwwroot/tailwind.generated.css` は生成物です。直接編集せず、
  `npm run css:build` または `npm run css:watch` で更新してください。

## 設計・開発規約

- 設計判断と理由: [Architecture Decision Records](docs/adr/README.md)
- エージェントを含む開発規約: [AGENTS.md](AGENTS.md)
