# ADR-0010: Web を実行ホストとし Api をエンドポイント定義ライブラリとして残す

- ステータス: 承認済み
- 日付: 2026-08-25
- 関連: [Issue #16](https://github.com/Ries630/BlazorCleanShop/issues/16)

## 背景

[ADR-0001](0001-host-blazor-and-minimal-apis-together.md) では、Blazor UI と Minimal APIs を
同一の ASP.NET Core ホストで実行すると決定した。一方、どのプロジェクトを実行ホストに
するか、`BlazorCleanShop.Api` を残すか、エンドポイントをどこに配置するかは決めていなかった。

判断時点では、`BlazorCleanShop.Web` と `BlazorCleanShop.Api` がどちらも実行可能な
Web プロジェクトである。Web は Blazor、Identity、SQLite とミドルウェアの構成を持ち、
Api は OpenAPI の構成と WeatherForecast サンプルを持つ。

このプロジェクトでは、クリーンアーキテクチャにおける入力アダプターの分離と
Minimal API の設計を学習対象としている。

## 決定

`BlazorCleanShop.Web` を唯一の実行ホスト兼構成ルートとする。
`BlazorCleanShop.Api` は Minimal API エンドポイントを定義するクラスライブラリとして残す。

実行時の DI、ミドルウェア、OpenAPI と Scalar の構成は Web に置く。Api は Application に
依存し、Web は Api、Application、Infrastructure を組み立てる。Blazor コンポーネントは
Application を直接呼び、同一ホストの API へ自己 HTTP 通信しない。

## 検討した代替

### Web にエンドポイントも配置して Api を削除する

実行プロジェクトが 1 つになり、プロジェクト参照も最小になる。しかし、Blazor UI と
Minimal API という 2 種類の入力アダプターの境界がプロジェクト構成から見えなくなり、
API 設計を独立して学ぶ目的に合わないため採用しない。

### Api を実行ホストにして Web を Razor クラスライブラリにする

同一ホスト構成にはできる。しかし、現在 Web が持つ Blazor、Identity、SQLite、静的資産、
ミドルウェアの実行構成を Api へ移す必要がある。また、UI も配信する実行プロジェクトが
`Api` という名前になり、責務を誤解しやすいため採用しない。

### 新しい Host プロジェクトから Web と Api を参照する

実行構成と 2 つの入力アダプターを物理的に分離できる。しかし、現在の規模では
3 つの外側プロジェクトを管理する複雑さに見合う要件がないため採用しない。

## 結果

- Api を単独で起動、デプロイ、スケールできなくなる
- Web が Api を参照するため、Web は Blazor UI だけのプロジェクトではなく構成ルートも兼ねる
- Web にエンドポイントを直接置く案よりプロジェクト参照が 1 つ増える
- Api を独立ホストへ戻す場合は、新しい起動処理、認証、設定、デプロイ単位を用意する必要がある

## 再評価のサイン

- API を Web と別の周期でリリース、デプロイ、スケールする要件が生じた
- Api が薄い転送コードだけになり、プロジェクトとして分ける学習上の価値がなくなった
- 複数の UI または API モジュールを組み合わせるため、独立した構成ルートが必要になった
