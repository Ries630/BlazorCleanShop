# ADR-0003: Minimal APIs と OpenAPI、Scalar を採用する

- ステータス: 承認済み
- 日付: 2026-03-29
- 関連: [Issue #3](https://github.com/Ries630/BlazorCleanShop/issues/3)、[ADR-0001](0001-host-blazor-and-minimal-apis-together.md)、[当初の設計会話](https://claude.ai/share/081f3e76-b272-49f4-9f1c-de15f8b0b5d4)

## 背景

API 設計はこのプロジェクトの主な学習目的である。Blazor Interactive Server は
Application Service を直接呼び出せるため、Web UI の実装だけなら HTTP API は必須ではない。
一方で、API 設計そのものの学習、将来の外部クライアント、Blazor WebAssembly への変更が
API を設ける理由として挙がっていた。

API ドキュメントについては、Apidog と ASP.NET Core に統合される OpenAPI 生成を比較した。
Apidog はテストクライアントとドキュメントを一体化できるが、コードから生成する OpenAPI
仕様との二重管理になり得る。ASP.NET Core 側では XML ドキュメントコメントと
エンドポイントのメタデータから仕様を生成できる。

## 決定

HTTP API は Controller ではなく Minimal APIs で実装する。OpenAPI 仕様は
`Microsoft.AspNetCore.OpenApi` でコードから生成し、ドキュメント UI には Scalar を使う。
Apidog は仕様の正として導入しない。

API と Blazor UI の実行ホストに関する判断は [ADR-0001](0001-host-blazor-and-minimal-apis-together.md)
に従う。Minimal API エンドポイントを配置する物理プロジェクトは、この ADR では決定しない。

## 検討した代替

### HTTP API を設けない

Blazor Component から Application Service を直接呼ぶだけなら最も単純になる。しかし、
API 設計と OpenAPI ドキュメントの学習機会がなくなるため採用しない。

### Controller ベースの API

ASP.NET Core MVC の経験を再利用できる。しかし、最新の ASP.NET Core で簡潔な API を
設計するという学習方針に対して、Minimal APIs を選んだため採用しない。

### Apidog を API 仕様の正にする

API テストとドキュメントを一つのツールで扱える。しかし、C# コードから生成する OpenAPI
仕様と別に管理することになるため採用しない。

### Swashbuckle と Swagger UI を使う

既存の ASP.NET Core プロジェクトで広く利用されている。一方、このプロジェクトは
.NET 10 の新規プロジェクトとして Microsoft の OpenAPI パッケージと Scalar を選んだ。

## 結果

- Controller、Action、Controller 固有の規約やフィルターを前提にした実装はできない
- Apidog が提供する設計・テスト・ドキュメントの統合機能は標準の作業手順に含まれない
- OpenAPI の内容は、エンドポイントのメタデータと XML ドキュメントコメントの品質に依存する
- Blazor UI は Minimal API を経由しないため、UI 操作だけでは API 契約を検証できない
- 別の API 方式やドキュメント UI に変更する場合、エンドポイント定義、メタデータ、
  パッケージと公開設定の変更が必要になる

## 再評価のサイン

- 外部の利用者と、実装前の API 仕様を共同編集する必要が生じた
- Minimal API のエンドポイント定義では共通処理や構成を把握しにくくなった
- Microsoft の OpenAPI または Scalar に関する推奨方針が変わった
- 現在の構成では必要な API テスト、モック、ドキュメント公開機能を満たせなくなった
