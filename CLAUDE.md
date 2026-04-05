# CLAUDE.md - blazor-clean-shop

## プロジェクト概要

プログラミング学習用のデモショッピングサイト。

### 学習の主目的

1. **デザインパターン** — クリーンアーキテクチャ、軽量DDD、Repositoryパターン等の実践
2. **API設計** — Minimal APIs + OpenAPI仕様によるRESTful API設計
3. **ユニットテスト** — xUnit v3 + Moq によるTDD
4. **GitHub活用** — Issue駆動の開発、PR運用、CI/CD（GitHub Actions）
5. **AIツール活用** — Claude Code、GitHub Copilot（無料版）の併用

## 技術スタック

- **ランタイム**: .NET 10 (LTS)
- **言語**: C# 14
- **フレームワーク**: ASP.NET Core Blazor Web App
- **レンダリングモード**: Interactive Server
- **ORM**: Entity Framework Core 10
- **DB**: SQLite（開発用）
- **テスト**: xUnit v3 (`xunit.v3.mtp-v2`) + Moq — Microsoft Testing Platform v2
- **API仕様**: Microsoft.AspNetCore.OpenApi + Scalar

## アーキテクチャ: クリーンアーキテクチャ

依存の方向は常に **外 → 内**（Web → Application → Domain）。
Domain層は他のどのプロジェクトも参照しない。
各層は独立した `.csproj` プロジェクトとし、プロジェクト参照でコンパイラが依存方向を強制する。

```
BlazorCleanShop/
├── BlazorCleanShop.sln
├── src/
│   ├── BlazorCleanShop.Domain/           # エンティティ, Value Object, リポジトリIF
│   ├── BlazorCleanShop.Application/      # ユースケース（サービス層）, DTO
│   ├── BlazorCleanShop.Infrastructure/   # EF Core実装, リポジトリ実装
│   ├── BlazorCleanShop.Api/              # ASP.NET Core Web API（Minimal APIs）
│   └── BlazorCleanShop.Web/              # Blazor Web App（UI層、APIを呼び出す）
└── tests/
    └── BlazorCleanShop.Tests/            # xUnit v3 (MTP v2) + Moq
```

### プロジェクト参照の方向

```
Domain        ← 参照なし（完全独立）
Application   → Domain
Infrastructure → Domain, Application
Api           → Application, Infrastructure（DI登録のため）
Web           → HttpClientでAPIを呼ぶだけ（他プロジェクトへの直接参照は最小）
Tests         → テスト対象のプロジェクトを参照
```

### 通信フロー

```
Browser → SignalR → Blazor Component → HttpClient → API → Application Service → Domain
```

Blazor ServerはServiceを直接DIせず、HttpClient経由でAPI層を呼び出す。
これによりAPI層が単一のエントリーポイントとなり、将来のモバイル対応等にも対応可能。

## 採用するデザインパターン

- **Repository パターン** — Domain層にインターフェース、Infrastructure層に実装
- **Entity / Value Object** — DDDの軽量採用。識別子を持つものはEntity、持たないものはValue Object
- **ユースケース層の分離** — Application層にビジネスロジックを集約
- **DI（依存性注入）** — ASP.NET Core 組み込みDIを活用、依存性逆転の原則
- **TDD** — ドメイン層・ユースケース層を中心にxUnit v3 + Moqでテスト

## 意図的に採用しないパターン（過剰複雑化の回避）

- CQRS
- MediatR
- Domain Events
- Specification パターン

これらは小規模アプリでは学習コスト対リターンが薄いため除外。
必要になった時点で段階的に導入を検討する。

## 想定機能スコープ

- 商品一覧・詳細
- カート機能
- 注文機能
- （認証はIdentityテンプレートで最小限）

## レイヤー別の責務と構成要素

### Domain層（BlazorCleanShop.Domain）

- **Entity**: ビジネスルールを持つ。貧血モデル（ただのデータ入れ物）にしない
  - ファクトリメソッド（`Order.Create()`）で生成ルールをEntity自身が持つ
  - 状態変更もEntity内のメソッドで行う（`order.Cancel()`）
- **Value Object**: 識別子を持たず値で等価判定。C# の `record` で実装
  - 例: `Money`, `Address`, `Email` など、primitiveをそのまま使わない
- **Repository インターフェース**: `IOrderRepository` など。実装はInfrastructure層
- **DomainException**: ドメイン固有の例外

### Application層（BlazorCleanShop.Application）

- **Service（ユースケース）**: ビジネスロジックのオーケストレーション
  - 例: `OrderService.CreateOrder()` — リポジトリ呼び出し + Entityのファクトリメソッド呼び出し
- **DTO**: 層の境界をまたぐデータ運搬用。ロジックなし
  - 例: `OrderDto` — UIに返すためのフラットなデータ構造
  - Entity ≠ DTO を厳守する

### Infrastructure層（BlazorCleanShop.Infrastructure）

- **Repository 実装**: EF Core を使った `IOrderRepository` の実装
- **DbContext**: EF Core の `AppDbContext`
- **マッピング設定**: Entity ↔ DB テーブルの設定

### Api層（BlazorCleanShop.Api）

- **Minimal APIs**: エンドポイント定義（.NET 10 推奨スタイル）
- Application層のServiceをDIで受け取り、HTTPリクエストを橋渡しする
- **API仕様**: Microsoft.AspNetCore.OpenApi + Scalar でOpenAPIドキュメントを自動生成
- XMLドキュメントコメントがそのままAPI仕様書に反映される

### Web層（BlazorCleanShop.Web）

- Blazor Web App（Interactive Server モード）
- 空テンプレート（`-e`）から作成、デフォルトの Bootstrap は使用しない
- HttpClient経由でAPI層を呼び出す（Serviceを直接DIしない）
- DI登録（Program.cs）

## UI方針

- **CSSフレームワーク**: Tailwind CSS + daisyUI
- **データグリッド**: QuickGrid（Microsoft公式、`Microsoft.AspNetCore.Components.QuickGrid`）
  - 商品一覧・注文一覧等のテーブル表示に使用
  - daisyUIのクラスを `RowClass` 等で併用可
- Blazor本来の開発体験（Razorコンポーネント、CSS Isolation等）を重視しつつ、スタイリングにdaisyUIを活用
- Blazor専用UIライブラリ（MudBlazor等）やCommunityToolkit.Mvvmは採用しない
- コンポーネント固有のスタイルは `.razor.css`（CSS Isolation）を併用可
- .NET 10 / Blazor の新機能は積極的に採用する
  - `[PersistentState]` による状態永続化
  - `NavigationManager.NotFound()` による404ハンドリング
  - QuickGrid の `RowClass`、`HideColumnOptionsAsync()` 等

## コメント・ドキュメント方針

- コメントは **日本語** で記載する
- **XMLドキュメントコメント（`///`）を重点的に必ず記載する**
  - すべてのpublicクラス、メソッド、プロパティに付与
  - `<summary>`, `<param>`, `<returns>`, `<exception>` を適切に使う
  - API層ではこれがOpenAPI仕様書に直結する
- インラインコメントは「なぜ（Why）」を中心に書く

## API設計方針

- **Minimal APIs** スタイルで実装（Controllerベースは採用しない）
- **OpenAPI仕様**: `Microsoft.AspNetCore.OpenApi` で自動生成
- **APIドキュメントUI**: Scalar（.NET 10推奨、Swashbuckle後継）
- エンドポイントのXMLドキュメントコメントが仕様書に反映されるため、コメントを丁寧に書く

## テスト方針

- **テストフレームワーク**: xUnit v3 (パッケージ: `xunit.v3.mtp-v2`)
- **テストランナー**: Microsoft Testing Platform v2（VSTestは使用しない）
- **モック**: Moq
- **テスト実行**: `dotnet test`（MTPモード）または `dotnet run` で直接実行
- テストプロジェクトの `OutputType` は `Exe`（MTP要件）
- Domain層・Application層のユニットテストを重点的に書く

## コーディング方針

- 不必要に複雑にしない。実務で使いやすい範囲で設計パターンを取り入れる
- 具体的・実装寄りの説明を好む（抽象的な概念説明より実コード）
- C# の命名規則に従う（PascalCase for public, _camelCase for private fields）
- **.NET 10 / C# 14 の新機能・新APIは積極的に採用する**（レガシーな書き方より最新の推奨パターンを優先）

## 実装の分担・ツール活用

- **自分で実装する**: Domain層、Application層、Infrastructure層、Api層、Tests
- **Claudeにお任せする**: Web層（Blazor UI）— UI層にはさほど興味がないため
- Claudeへの依頼時は、既存のアーキテクチャ・命名規則・コメント方針に従うこと
- **GitHub Copilot（無料版）**: コード補完・テストコード生成など日常的に活用する

## GitHub運用方針

- **Issue駆動**: 機能追加・バグ修正はIssueを作成してから着手する
- **ブランチ戦略**: GitHub Flow — `main` + feature branch（`feature/xxx`、`fix/xxx`）
- **PR運用**: feature branch → main へのPRを作成し、セルフレビューを習慣化
- **CI/CD**: GitHub Actions でビルド・テスト自動実行（PRトリガー）

## 将来の拡張方針

将来的に外部API連携・マイクロバッチ（ファイル連携）を追加予定。
現時点でプロジェクトは作らないが、以下を意識して設計する。

### 設計上の原則

- **Application層のServiceはHTTP非依存にする**: 引数・戻り値はDTO/値で統一し、HttpRequestやIResult等に依存しない
- **I/Oインターフェースの置き場所を守る**: ファイル出力・外部APIクライアント等のインターフェースはApplication層に、実装はInfrastructure層に置く
- **新しいエントリーポイントはプロジェクト追加で対応**: Application層を共通基盤として、新たなホスト（Batch、外部連携）を足すだけで拡張できる構成を維持する

### 将来追加するプロジェクトのイメージ

```
src/
├── BlazorCleanShop.Batch/            # コンソールアプリ（マイクロバッチ、ファイル連携）
└── BlazorCleanShop.ExternalApi/      # 外部API連携クライアント
```

いずれも Application層の既存Serviceを呼び出す形で実装する。
