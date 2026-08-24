# Architecture Decision Records

このプロジェクトの設計判断を、判断した時点の記録として残す場所。

自動で読み込まれる文書（AGENTS.md）には**結論**だけを置く。そこに無いと同じ提案が
蒸し返される。一方で**理由・却下した代替・その時点の測定値**は必要になったときに
読めればよく、しかも後から上書きされては困る。この 2 つを分けるのが ADR の役目。

## 運用の 3 ルール

1. **一度書いたら本文は編集しない。** 判断が変わったら新しい ADR を書き、古い方は
   ステータス行を `廃止（ADR-XXXX により置換）` に変えるだけにする。本文を
   書き換えると「当時なにを知らなかったのか」が消える
2. **Issue → ADR → PR の順。** Issue で迷い、決まったら ADR を実装の PR に同梱する
3. **連番。** 欠番を作らない

## 何を ADR にするか

| 基準 | ADR にする | 書かない |
|---|---|---|
| 取り消しコスト | 高い（データ移行・API 互換が要る） | いつでも戻せる |
| 代替の比較 | 実際に他案を検討して落とした | 慣習の採用 |
| 再評価のサイン | 「こうなったら見直す」条件がある | ない |
| 守られ方 | 文書でしか守られない | テスト・型・lint が自動的に守る |

最後の行が効く。破ればテストが落ちる取り決めは書かない。書いても二重管理になり、
しかも文書の方が先に古くなる。

## 意図的に書いていないもの

上の表で「書かない」と判断したものを、**代わりに何が守っているか**とセットで残す。
ここに無いと「なぜ ADR が無いのか」が次に同じ話題が出たときにまた蒸し返される。

- 現時点ではなし

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
| [0008](0008-use-dependabot-with-ecosystem-groups.md) | 依存更新に Dependabot を採用しエコシステムごとに集約する | 承認済み |

## テンプレート

[`template.md`](template.md) をコピーして使う。作成・更新は `adr` skill から。
