# ADR-0006: Blazor 標準コンポーネントに daisyUI と QuickGrid を組み合わせる

- ステータス: 承認済み
- 判断日: 2026-04-02
- 記録日: 2026-08-23
- 関連: [Issue #3](https://github.com/Ries630/BlazorCleanShop/issues/3)、[当初の設計会話](https://claude.ai/share/081f3e76-b272-49f4-9f1c-de15f8b0b5d4)

## 背景

りーすさんは daisyUI を好んでおり、UI 実装自体を主な学習対象にはしていない。一方で、
Razor Component、Blazor の状態更新、CSS Isolation など、Blazor 本来の開発体験は
維持する意向がある。

UI の選択肢として、テンプレート標準の Bootstrap、MudBlazor、Radzen、Blazorise、
Fluent UI Blazor などの Blazor 用コンポーネントライブラリ、Tailwind CSS と daisyUI の
直接利用を比較した。また、WPF や MAUI で利用経験のある CommunityToolkit.Mvvm と、
Microsoft 公式の QuickGrid についても検討した。

## 決定

Blazor Web App は空テンプレートから開始し、Bootstrap は使用しない。スタイリングには
Tailwind CSS と daisyUI を直接使用し、Razor Component と CSS Isolation を維持する。
一覧表示には Microsoft 公式の QuickGrid を使用する。

MudBlazor などの包括的な Blazor UI ライブラリ、CommunityToolkit.Mvvm、Blazing.Mvvm
などの MVVM レイヤーは導入しない。

## 検討した代替

### テンプレート標準の Bootstrap を使う

追加の CSS ビルド環境が不要になる。しかし、好みの daisyUI を使い、サンプルの見た目を
引き継がずに UI を作る方針を選んだため採用しない。

### MudBlazor、Radzen、Blazorise、Fluent UI Blazor を使う

Blazor 向けの高機能なコンポーネントを利用できる。しかし、ライブラリ固有のコンポーネント
モデルを学ぶ範囲が増え、標準の Razor と HTML クラスを中心にする方針から外れるため採用しない。

### CommunityToolkit.Mvvm または Blazing.Mvvm を使う

WPF や MAUI に近い MVVM の記述ができる。しかし、Blazor はコンポーネントの再レンダリングを
基本とし、MVVM Toolkit が解決する変更通知の問題を同じ形では持たないため採用しない。

### Blazor.DaisyUI のラッパーを使う

daisyUI の要素を Razor Component として利用できる。しかし、検討時点ではベータ版であり、
daisyUI の CSS クラスを直接使う方法で要件を満たせるため採用しない。

## 結果

- Bootstrap 向けのサンプル、スタイル、コンポーネントは利用できない
- Tailwind CSS と daisyUI のために npm を使う CSS ビルド環境が必要になる
- 高機能な Blazor UI ライブラリが提供する完成済みコンポーネントを利用できない
- UI の構造、状態管理、アクセシビリティ、daisyUI のクラス指定を Razor 側で実装する必要がある
- CSS クラスの誤りや daisyUI の変更は C# のコンパイルでは検出できない
- QuickGrid の範囲を超える表機能が必要な場合は、自作または別ライブラリの検討が必要になる

## 再評価のサイン

- 複雑なデータグリッドや入力部品を自作する作業が繰り返し発生した
- UI のアクセシビリティや一貫性を、現在の構成では維持できなくなった
- Tailwind CSS のビルド環境が Blazor の開発・配布を妨げるようになった
- Blazor 向け daisyUI ラッパーが安定し、直接利用より明確な利点を持つようになった
- Blazor 本来のコンポーネントモデルを使う方針自体を見直すことになった
