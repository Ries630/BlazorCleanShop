# ADR-0007: プロジェクト指示の正を AGENTS.md に置く

- ステータス: 承認済み
- 日付: 2026-08-23
- 関連: [Issue #5](https://github.com/Ries630/BlazorCleanShop/issues/5)

## 背景

プロジェクト指示はルートの `CLAUDE.md` だけにあり、Claude Code を前提としたファイル名と
表現になっている。Codex など `AGENTS.md` を探索するコーディングエージェントにも、同じ
アーキテクチャ、実装、テストの方針を読み込ませる必要がある。

一方、Claude Code でも引き続き開発する。Claude Code Desktop の Preview は、Blazor の
HTTP・HTTPS開発サーバー設定を `.claude/launch.json` から読み込んでいる。

## 決定

ルートの `AGENTS.md` を共有するプロジェクト指示の正とする。`CLAUDE.md` は
`@AGENTS.md` だけを記載するClaude Code向けのインポートにする。

`.claude/launch.json` は共有指示ではなくClaude Code Desktop固有のPreview設定なので、
互換アダプターとして維持する。過去のADRにあるClaude共有リンクも当時の一次資料として維持する。

## 検討した代替

### CLAUDE.md と AGENTS.md に同じ指示を複製する

どちらのエージェントも直接ファイルを読める。しかし、更新のたびに二つの実体を同期する
必要があり、片方だけが古くなるため採用しない。

### CLAUDE.md を AGENTS.md へのシンボリックリンクにする

指示の実体は一つにできる。しかし、Windowsではシンボリックリンクの利用に追加設定が
必要になるため、通常ファイルによるインポートを採用する。

### Claude固有ファイルをすべて削除する

ベンダー名を含むファイルはなくなる。しかし、Claude Codeが共有指示を自動で読めず、
Claude Code DesktopのPreviewでも既存の開発サーバー設定を利用できなくなるため採用しない。

## 結果

- 共有指示をClaude Code専用の形式だけで記述できない
- Claude Codeの互換性には、ルートの `CLAUDE.md` を維持する必要がある
- Claude Code DesktopのPreviewを使う間は `.claude/launch.json` が残るため、リポジトリから
  ベンダー名を含むパスを完全には排除できない
- `AGENTS.md` を探索しないエージェントには、同じ正本を読むための個別の橋渡しが必要になる

## 再評価のサイン

- Claude Codeがルートの `AGENTS.md` を直接探索するようになった
- 複数のコーディングエージェントが別の共通指示ファイルを標準として採用した
- Claude Code DesktopのPreviewを使わなくなった、または `launch.json` の形式が廃止された
- Claude Codeだけに適用するプロジェクト指示が必要になった
