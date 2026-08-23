# ADR-0001: Blazor UI と Minimal APIs を同一 ASP.NET Core ホストで実行する

- ステータス: 承認済み
- 日付: 2026-08-23
- 関連: [Issue #1](https://github.com/Ries630/BlazorCleanShop/issues/1)、[当初の設計会話](https://claude.ai/share/081f3e76-b272-49f4-9f1c-de15f8b0b5d4)

## 背景

当初は API 設計を学ぶため、Blazor Interactive Server の UI から `HttpClient` で
別プロセスの Minimal API を呼び出す構成を選んだ。将来のモバイルアプリ対応や
Blazor WebAssembly への変更も、API を設ける理由として挙がっていた。

一方、現時点では Web と API を別チームで開発する、別々の周期でリリースする、
または個別にスケールさせる要件はない。別プロセスにすると、同じユースケースの呼び出しが
JSON のシリアライズ、HTTP 通信、デシリアライズを経由し、実行・設定・障害点も 2 つになる。

学習の中心は、リッチドメインモデル、クリーンアーキテクチャ、Minimal APIs、TDD、
GitHub 運用である。TypeScript はまだ読み慣れておらず、C# で UI を記述する体験にも
関心がある。ただし、Blazor 自体を学習の中心にはしない。

## 決定

Blazor Interactive Server の UI と Minimal APIs を、1 つの ASP.NET Core ホストとして
実行・デプロイする。Blazor コンポーネントと Minimal API エンドポイントは、それぞれ
Application 層を直接呼び出す入力アダプターとする。

Minimal APIs による API 設計と OpenAPI ドキュメントの学習は継続する。Blazor UI から
同じプロセスの API へ自己 HTTP 通信は行わない。

どのプロジェクトを実行ホストにするか、既存の `BlazorCleanShop.Api` を独立した
プロジェクトとして残すか、Minimal API エンドポイントをどこに配置するかは、
この ADR では決定しない。

## 検討した代替

### Blazor Interactive Server と Minimal API を別プロセスにする

API を独立してデプロイ・リリース・スケールでき、Web 以外のクライアントとも共有しやすい。
しかし、現時点ではその独立性を必要とする要件がなく、Blazor UI が API をそのまま中継して
呼ぶだけでは、通信と運用の複雑さに見合う利点がないため採用しない。

### TypeScript の SPA から Minimal API を呼ぶ

一般的なブラウザ SPA と HTTP API の構成を経験でき、フロントエンドの知識を転用しやすい。
しかし、TypeScript とフロントエンドフレームワークを同時に学ぶと現在の学習対象が増え、
バックエンド設計から焦点が外れるため採用しない。

### Blazor WebAssembly から Minimal API を呼ぶ

C# で UI を記述しながら、ブラウザから API を直接呼ぶ SPA に近い構成にできる。
しかし、クライアント側で .NET を実行する要件はなく、現在の目的には Interactive Server と
同一ホストの方が単純なため採用しない。

## 結果

- Web と API を別々にデプロイ、リリース、スケールできない
- Blazor UI は HTTP 境界を通らないため、UI の操作だけでは Minimal API の契約を検証できない
- Web と API の実行設定、認証、障害境界を独立させられない
- 将来ホストを分離する場合、API の起動構成、認証、URL 設定と Blazor 側の
  `HttpClient` 呼び出しを追加する必要がある
- このプロジェクトでは TypeScript を使ったフロントエンド開発を学べない

## 再評価のサイン

- Web と API を別チーム、別リリース周期、または別スケール単位で運用する必要が生じた
- モバイルアプリや外部クライアント向け API を、Web と独立して提供する必要が生じた
- オフライン動作など、ブラウザ側で UI ロジックを実行する要件が生じた
- TypeScript を使った一般的な SPA 開発が、このプロジェクトの明示的な学習目的になった
- Blazor の試作後、固有の実行モデルや開発体験が目的に合わないと判断した
