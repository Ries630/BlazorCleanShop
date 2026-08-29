# ADR-0012: Shopifyを基本購入フローの参照モデルとする

- ステータス: 承認済み
- 日付: 2026-08-29
- 関連: [Issue #39](https://github.com/Ries630/BlazorCleanShop/issues/39)

## 背景

基本購入フローの後続Issueでは、商品、カート、注文を個別に実装する前に、表示上の商品と
購入単位、価格の確定時点、カートと注文の関係を一貫させる必要があった。

Shopifyでは、ProductVariantがProductに属する購入単位として価格を持ち、CartLineは
ProductVariantと数量を表す。Cartの金額は見積もりであり、注文のLineItemには注文作成時の
商品名と単価が残る。この関係は、Issue #39で合意した基本購入フローに適合する。

- [ProductVariant](https://shopify.dev/docs/api/admin-graphql/latest/objects/ProductVariant)
- [Cartの関係](https://shopify.dev/docs/storefronts/headless/building-with-the-storefront-api/cart/manage)
- [注文のLineItem](https://shopify.dev/docs/api/admin-graphql/latest/objects/LineItem)

## 決定

Shopifyを基本購入フローの参照モデルとし、Product、ProductVariant、Cart、Orderの関係を
簡略化して採用する。ShopifyのAPIと機能一式は仕様にせず、このリポジトリの
[`CONTEXT.md`](../../CONTEXT.md) と
[`docs/basic-purchase-flow.md`](../basic-purchase-flow.md) を用語とビジネスルールの正とする。

Productを表示上の商品、ProductVariantを価格を持つ購入単位とする。CartLineは
ProductVariantと数量を参照し、OrderLineは注文時点の商品名、ProductVariant名、単価、
数量を保持する。

## 検討した代替

### Product自体を購入単位にする

Entityとテーブルを減らせる。しかし、表示上の商品と価格を持つ購入単位が同一になり、
後からサイズや色を追加するとCartLineとOrderLineの参照先を変更する必要があるため採用しない。

### Shopifyのモデルを広く再現する

Checkout、在庫、決済、配送、税、割引、複数通貨、販売プランまで一貫したモデルを参照できる。
しかし、基本購入フローと学習目的に不要な状態と依存が増えるため採用しない。

### 外部の参照モデルを設けず独自に設計する

現在の要件だけに合わせられる。しかし、後続Issueごとに似た概念を異なる意味で設計する余地が
残り、今回の設計対話で比較したProductとProductVariantの区別を失うため採用しない。

## 受け入れた代償

- 選択肢がない商品にも「標準」のProductVariantが1件必要になる
- Productだけを持つ構成よりEntity、永続化、DTOの関係が増える
- Shopifyに存在する概念でも、ローカル文書に定義していない振る舞いは利用できない
- Shopify側のモデル変更を、このプロジェクトへ自動的に取り込むことはできない

## 再評価の条件

- ProductVariantを必要とする機能が長期間なく、分離の保守コストが学習効果を上回った
- 基本購入フローがShopifyと大きく異なる業務へ変わり、用語の対応が誤解を生むようになった
- 現在対象外のCheckout、決済、配送を導入し、参照する範囲を改めて決める必要が生じた
