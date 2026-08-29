# ADR-0013: IdentityとShoppingの永続化を分離する

- ステータス: 承認済み
- 日付: 2026-08-29
- 関連: [Issue #39](https://github.com/Ries630/BlazorCleanShop/issues/39)、[ADR-0011](0011-generate-identity-sqlite-locally.md)

## 背景

現在のWeb層には、ASP.NET Core Identity専用の`ApplicationDbContext`があり、
`Data/app.db`を使用している。Infrastructure層には業務データ用DbContextがまだなく、
商品、カート、注文の保存先とマイグレーション境界を決める必要がある。

ショッピングコンテキストは認証済み利用者のIDだけを必要とし、利用者情報を管理しない。
注文確定では注文追加とカート消費を同じトランザクションで行う必要があるが、Identityの
データを同時に変更する要件はない。

## 決定

IdentityはWeb層の既存`ApplicationDbContext`と`Data/app.db`、商品、カート、注文は
Infrastructure層の新しい`ShoppingDbContext`と`Data/shop.db`へ保存する。DbContext、
SQLiteファイル、マイグレーションをそれぞれ独立させる。

Shopping DBにはIdentityの利用者IDを不透明な値として保存し、Identity DBへの外部キーを
設けない。注文追加とカート消費は同じ`ShoppingDbContext`で原子的に確定し、Identity DBを
トランザクションへ参加させない。

## 検討した代替

### ApplicationDbContextへ業務テーブルを追加する

1つのDbContextとSQLiteファイルだけを管理でき、利用者への外部キーも作成できる。しかし、
Identityを構成するWeb層が業務データの永続化も所有し、Infrastructure層へRepository実装を
置くレイヤー境界と一致しないため採用しない。

### DbContextを分けて同じSQLiteファイルを共有する

IdentityとShoppingのモデルをコード上で分離しながら、DBファイルを1つにできる。しかし、
マイグレーション履歴、テーブル所有、適用順を同じDB内で調整する必要が生じる。現在は
IdentityとShoppingをまたぐ外部キーやトランザクションを必要としないため採用しない。

## 受け入れた代償

- Identityの利用者とCart、Orderの対応をDBの外部キーで保証できない
- DBファイルの生成、バックアップ、削除を2系統で扱う必要がある
- IdentityとShoppingを同時に更新する原子的なユースケースを追加できない
- 利用者削除時のCartとOrderの扱いはDBの削除連鎖ではなく、別の方針として決める必要がある

## 再評価の条件

- IdentityとShoppingを同時に更新するユースケースが必要になった
- 利用者と業務データの参照整合性をDBで強制する必要が生じた
- 本番向けDBの構成、移行、バックアップ方式を決める段階になった
- SQLite以外のDBへ移行し、スキーマやトランザクションの境界を再設計する必要が生じた
