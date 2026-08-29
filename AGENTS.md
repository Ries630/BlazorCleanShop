# AGENTS.md - blazor-clean-shop

## プロジェクト概要

プログラミング学習用のデモショッピングサイト。

### 学習の主目的

1. **デザインパターン** — クリーンアーキテクチャ、軽量DDD、Repositoryパターン等の実践
2. **API設計** — Minimal APIs + OpenAPI仕様によるRESTful API設計
3. **ユニットテスト** — xUnit v3 + Moq によるTDD
4. **GitHub活用** — Issue駆動の開発、PR運用、CI/CD（GitHub Actions）
5. **AIツール活用** — コーディングエージェント、GitHub Copilot（無料版）の併用

## エージェント向け指示の管理

この `AGENTS.md` をプロジェクト指示の正とする。Claude Code はルートの
`CLAUDE.md` からこのファイルを読み込む。理由と再評価条件は
[ADR-0007](docs/adr/0007-use-agents-md-as-canonical-instructions.md) を参照。

## 技術スタック

- **ランタイム**: .NET 10 (LTS)
- **言語**: C# 14
- **フレームワーク**: ASP.NET Core Blazor Web App
- **レンダリングモード**: Interactive Server
- **ORM**: Entity Framework Core 10
- **DB**: SQLite（開発用。Identity DB は Git 管理せず、マイグレーションから
  ローカル生成する）— [ADR-0011](docs/adr/0011-generate-identity-sqlite-locally.md)
- **テスト**: xUnit v3 (`xunit.v3.mtp-v2`) + Moq — Microsoft Testing Platform v2
- **API仕様**: Microsoft.AspNetCore.OpenApi + Scalar

## 依存関係の更新

依存更新には Dependabot を使用し、NuGet と npm を別々の PR にする。minor・patch 更新は
エコシステムごとの月次 PR に集約し、major 更新は個別 PR にする。通常更新ルールの正は
`.github/dependabot.yml` とし、自動マージは行わない。理由と再評価条件は
[ADR-0009](docs/adr/0009-separate-major-dependency-updates.md) を参照。

## アーキテクチャ: クリーンアーキテクチャ

依存の方向は常に **外 → 内**（Web → Application → Domain）。
Domain層は他のどのプロジェクトも参照しない。
各層は独立した `.csproj` プロジェクトとし、プロジェクト参照でコンパイラが依存方向を強制する。
軽量 DDD の採用範囲とプロジェクト分割の理由は
[ADR-0002](docs/adr/0002-use-clean-architecture-with-lightweight-ddd.md) を参照。

Blazor UI と Minimal APIs は**同一の ASP.NET Core ホスト**で実行する。Blazor
コンポーネントと API エンドポイントは、それぞれ Application 層を直接呼び出す。
理由と再評価条件は [ADR-0001](docs/adr/0001-host-blazor-and-minimal-apis-together.md) を参照。

```
BlazorCleanShop/
├── BlazorCleanShop.sln
├── src/
│   ├── BlazorCleanShop.Domain/           # エンティティ, Value Object, リポジトリIF
│   ├── BlazorCleanShop.Application/      # ユースケース（サービス層）, DTO
│   ├── BlazorCleanShop.Infrastructure/   # EF Core実装, リポジトリ実装
│   ├── BlazorCleanShop.Api/              # Minimal API エンドポイント定義（クラスライブラリ）
│   └── BlazorCleanShop.Web/              # Blazor UIと唯一の実行ホスト
└── tests/
    └── BlazorCleanShop.Tests/            # xUnit v3 (MTP v2) + Moq
```

`BlazorCleanShop.Web` を唯一の実行ホスト兼構成ルートとし、`BlazorCleanShop.Api` は
Minimal API エンドポイントを定義するクラスライブラリとして残す。実行時の DI、
ミドルウェア、OpenAPI と Scalar の構成は Web に置く。理由と再評価条件は
[ADR-0010](docs/adr/0010-use-web-host-with-api-endpoint-library.md) を参照。

### プロジェクト参照の方向

```
Domain        ← 参照なし（完全独立）
Application   → Domain
Infrastructure → Domain, Application
Api           → Application
Web（実行ホスト）→ Api, Application, Infrastructure
Tests         → テスト対象のプロジェクトを参照
```

### 通信フロー

```
Browser → SignalR → Blazor Component → Application Service → Domain
外部クライアント → HTTP → Minimal API → Application Service → Domain
```

Blazor Component と Minimal API は同一プロセス内の独立した入力アダプターとする。
Blazor から同じホストの API へ自己 HTTP 通信は行わない。

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

これらは現時点では採用しない。判断の理由と再評価条件は
[ADR-0002](docs/adr/0002-use-clean-architecture-with-lightweight-ddd.md) を参照。

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

### Minimal API

- **Minimal APIs**: エンドポイント定義（.NET 10 推奨スタイル）
- Application層のServiceをDIで受け取り、HTTPリクエストを橋渡しする
- **API仕様**: Microsoft.AspNetCore.OpenApi + Scalar でOpenAPIドキュメントを自動生成
- XMLドキュメントコメントがそのままAPI仕様書に反映される
- エンドポイントは `BlazorCleanShop.Api` に置き、実行時の構成は `BlazorCleanShop.Web` に置く

### Web層（BlazorCleanShop.Web）

- Blazor Web App（Interactive Server モード）
- 空テンプレート（`-e`）から作成、デフォルトの Bootstrap は使用しない
- Blazor ComponentはApplication層のServiceを直接DIする

## UI方針

- **CSSフレームワーク**: Tailwind CSS + daisyUI
- **データグリッド**: QuickGrid（Microsoft公式、`Microsoft.AspNetCore.Components.QuickGrid`）
  - 商品一覧・注文一覧等のテーブル表示に使用
  - daisyUIのクラスを `RowClass` 等で併用可
- Blazor本来の開発体験（Razorコンポーネント、CSS Isolation等）を重視しつつ、スタイリングにdaisyUIを活用
- Blazor専用UIライブラリ（MudBlazor等）は採用しない
- コンポーネント固有のスタイルは `.razor.css`（CSS Isolation）を併用可
- .NET 10 / Blazor の新機能は積極的に採用する
  - `[PersistentState]` による状態永続化
  - `NavigationManager.NotFound()` による404ハンドリング
  - QuickGrid の `RowClass`、`HideColumnOptionsAsync()` 等

UI 技術の選択と不採用案は
[ADR-0006](docs/adr/0006-use-native-blazor-with-daisyui-and-quickgrid.md) を参照。

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
- **APIドキュメントUI**: Scalar（Swagger UIの代替）
- エンドポイントのXMLドキュメントコメントが仕様書に反映されるため、コメントを丁寧に書く

API 実装・仕様生成・ドキュメント UI の選択は
[ADR-0003](docs/adr/0003-use-minimal-apis-with-openapi-and-scalar.md) を参照。

## テスト方針

- **テストフレームワーク**: xUnit v3 (パッケージ: `xunit.v3.mtp-v2`)
- **テストランナー**: Microsoft Testing Platform v2（VSTestは使用しない）
- **モック**: Moq
- **テスト実行**: `dotnet test`（MTPモード）または `dotnet run` で直接実行
- テストプロジェクトの `OutputType` は `Exe`（MTP要件）
- Domain層・Application層のユニットテストを重点的に書く

テスト実行基盤の選択理由と再評価条件は
[ADR-0004](docs/adr/0004-use-xunit-v3-with-mtp-v2.md) を参照。

## 実装の分担・ツール活用

- **自分で実装する**: Domain層、Application層、Infrastructure層、Minimal APIエンドポイント、Tests
- **コーディングエージェントに任せる**: Blazor UI — UI層にはさほど興味がないため
- エージェントへの依頼時は、既存のアーキテクチャ・命名規則・コメント方針に従うこと

## 将来の拡張方針

将来的に外部API連携・マイクロバッチ（ファイル連携）を追加予定。
現時点でプロジェクトは作らないが、以下を意識して設計する。
トランスポート非依存を保つ判断は
[ADR-0005](docs/adr/0005-keep-application-services-transport-independent.md) を参照。

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

## Codexレビュー指摘への応答

- Codex のレビューコメントへの返信で `@codex` がメンションされた場合、明示的な修正依頼が
  なければ、コードを変更せずレビュー指摘への対応状況を再判定する
- コードが修正されている場合は、現在の PR head を確認し、指摘した問題が解消されたかを
  判定する
- 説明だけが返信されている場合は、コード・Issue・リポジトリの規範と照合し、その説明が
  妥当かを判定する
- 結論は `解消` / `未解消` / `説明妥当` / `説明不十分` のいずれかで、日本語で根拠を添える
- 判定結果と根拠は同じレビュースレッドへ返信せず、Codex GitHub 連携が表示するタスクの
  最終報告に記載する
- `解消` または `説明妥当` と判定した場合だけ Resolve し、`未解消` または `説明不十分` の
  場合は Resolve しない

## Code Review Rules

- レビューコメントは日本語で記載する
