# Architecture Decision Records

コードから理由を読み取れない長期的な設計判断を、判断した時点の記録として残す。

## 運用

ADR の作成基準・書式・作成・更新・置換・廃止の手順は `adr` skill を正とする。
このファイルには、このリポジトリの ADR 一覧と動かない結論だけを置く。
用語とコンテキスト境界は `domain-modeling` skill を正とする。`CONTEXT.md` は同 skill が
必要時に作成・更新し、ADR には用語定義を重複して書かない。

## 一覧

| # | 決定 | ステータス |
|---|---|---|
| [0001](0001-host-blazor-and-minimal-apis-together.md) | Blazor UI と Minimal APIs を同一 ASP.NET Core ホストで実行する | 承認済み |
| [0002](0002-use-clean-architecture-with-lightweight-ddd.md) | クリーンアーキテクチャと軽量 DDD を採用する | 承認済み |
| [0003](0003-use-minimal-apis-with-openapi-and-scalar.md) | Minimal APIs と OpenAPI、Scalar を採用する | 承認済み |
| [0004](0004-use-xunit-v3-with-mtp-v2.md) | .NET 10 と xUnit v3、Microsoft Testing Platform v2 を採用する | 承認済み |
| [0005](0005-keep-application-services-transport-independent.md) | Application Service をトランスポート非依存に保つ | 承認済み |
| [0006](0006-use-native-blazor-with-daisyui-and-quickgrid.md) | Blazor 標準コンポーネントに daisyUI と QuickGrid を組み合わせる | 承認済み |
| [0007](0007-use-agents-md-as-canonical-instructions.md) | プロジェクト指示の正を AGENTS.md に置く | 承認済み |
| [0008](0008-use-dependabot-with-ecosystem-groups.md) | 依存更新に Dependabot を採用しエコシステムごとに集約する | 廃止（ADR-0009 により置換） |
| [0009](0009-separate-major-dependency-updates.md) | major 更新を月次グループから分離する | 承認済み |
| [0010](0010-use-web-host-with-api-endpoint-library.md) | Web を実行ホストとし Api をエンドポイント定義ライブラリとして残す | 承認済み |
| [0011](0011-generate-identity-sqlite-locally.md) | Identity 用 SQLite DB をマイグレーションからローカル生成する | 承認済み |
| [0012](0012-use-shopify-as-reference-model.md) | Shopify を基本購入フローの参照モデルとする | 承認済み |
| [0013](0013-separate-identity-and-shopping-storage.md) | Identity と Shopping の永続化を分離する | 承認済み |

## テンプレート

[`template.md`](template.md) をコピーして使う。作成・更新は `adr` skill から。
