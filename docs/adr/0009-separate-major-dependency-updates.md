# ADR-0009: major 更新を月次グループから分離する

- ステータス: 承認済み
- 日付: 2026-08-24
- 関連: [Issue #14](https://github.com/Ries630/BlazorCleanShop/issues/14)、[PR #23](https://github.com/Ries630/BlazorCleanShop/pull/23)、[ADR-0008](0008-use-dependabot-with-ecosystem-groups.md)

## 背景

最初の月次 NuGet PR である PR #23 には、Microsoft 系 5 パッケージの
`10.0.5` から `10.0.11` へのパッチ更新と、`xunit.v3.mtp-v2` の `3.2.2` から
`4.0.0` へのメジャー更新が同時に含まれた。

現時点のテストは `Fact` 1 件であり、PR #23 の内容で restore、build、test は成功した。
プロジェクトの実装がほぼ空である今回は混在を許容できるが、今後機能とテスト基盤が
育った状態では、メジャー更新をパッチ更新に混ぜると影響範囲を見落としやすくなる。

## 決定

Dependabot の月次グループには minor・patch 更新だけを含め、major 更新は個別 PR にする。
PR #23 にすでに含まれている xUnit v4 更新は今回限りの例外として受け入れる。

## 検討した代替

### major・minor・patchを同じ月次PRに含め続ける

通常更新 PR を NuGet と npm の最大 2 本に抑えられる。しかし、プロジェクトが育った後も
破壊的変更を含み得るメジャー更新が他の更新に埋もれるため採用しない。

### major更新をDependabotの対象外にする

通常更新 PR を最大 2 本に保てる。しかし、major 更新の存在を個別 PR で把握できず、
手動確認を忘れる可能性があるため採用しない。

### PR #23からxUnit v4更新を除外して作り直す

パッチ更新だけの PR にできる。しかし、現時点では実装がほぼ空で、`Fact` 1 件を含む
build と test が xUnit v4 で成功しているため、今回の PR を作り直す必要はないと判断した。

## 結果

- major 更新がある月は、通常更新 PR を NuGet と npm の合計 2 本以内には抑えられない
- 同じエコシステムで複数の major 更新があると、個別 PR が複数作成される
- minor・patch 更新と major 更新を同じ PR で一括確認することはできない
- PR #23 は例外として、xUnit のメジャー更新と Microsoft 系パッケージのパッチ更新を含む

## 再評価のサイン

- 個別の major 更新 PR が継続的に多くなり、レビュー負担が月次集約の利点を上回った
- major 更新を個別 PR にしても影響範囲の確認漏れが発生した
