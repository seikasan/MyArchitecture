# MyArchitecture

MyArchitecture は、Unity 向けの軽量アーキテクチャです。

View / Presenter / GameService / Model / Utility と分け、状態変更は Command、取得は Query、通知は Event で扱います。

QFramework の層構造や Command / Query の考え方を参考にしつつ、自分の Unity 開発で扱いやすい形に整理しています。

現在は開発途中です。  
API、命名、ディレクトリ構成は変更される可能性があります。

## コンセプト

MyArchitecture が目指しているのは、Unity の MonoBehaviour に処理が集まりすぎる状態を避けることです。

画面表示、入力、演出は View に置きます。
画面と状態の橋渡しは Presenter に置きます。
ゲームルールや状態変更は GameService と Command に置きます。
状態は Model に置きます。
共通処理や外部 API のラップは Utility に置きます。

その結果、次のような分け方をしやすくします。

- UI ボタンを押したとき、View は入力だけを通知する
- Presenter は入力を受け取り、必要な Command を送る
- Command は GameService や Model を通して状態を変える
- 状態の変化は ReactiveProperty や Event で Presenter に戻る
- Presenter は View の公開 API を呼んで表示を更新する

## 基本の流れ

```text
View input
→ ViewSignal / Observable
→ Presenter
→ Command
→ GameService / Model

Model change / Event
→ Presenter
→ View update
````

## 層構造

```text
Presenter + View
GameService
Model
Utility
```

上の層は下の層を利用できます。
下の層は上の層を知りません。

下位層から Presenter へ何かを伝える場合は Event を使います。
状態を変える場合は Command、状態を読むだけなら Query を使います。

## 各層の役割

| 層           | 役割                                                |
| ----------- | ------------------------------------------------- |
| View        | Unity の表示、入力、演出を扱う                                |
| Presenter   | View、ReadOnlyModel、Event を購読し、Command / Query を送る |
| GameService | ゲームロジックと Model の変更を担当する                           |
| Model       | ゲーム状態を持つ                                          |
| Utility     | 計算、変換、保存、読み込み、外部 API のラップなどを担当する                  |
| Command     | 状態変更を表す小さな処理単位                                    |
| Query       | 状態取得や計算結果の取得を表す小さな処理単位                            |
| Event       | 下位層から Presenter へ通知するメッセージ                                |

## ルール

* View は Presenter を直接参照しない
* Presenter は GameService を直接参照しない
* Presenter から状態を変えるときは Command を送る
* Presenter が状態を読むときは ReadOnlyModel または Query を使う
* Model は View / Presenter / GameService を知らない
* Model の書き換えは GameService または Command 経由にする
* 下位層から Presenter への通知は Event を使う
* Event は命令ではなく通知として扱う
* View の Tween や Animation は View 層に閉じ込める

## 使用ライブラリ

| ライブラリ       | 用途                                     |
| ----------- | -------------------------------------- |
| [VContainer](https://github.com/hadashiA/VContainer)  | DI、Lifetime 管理、Command / Query の組み立て   |
| [MessagePipe](https://github.com/Cysharp/MessagePipe) | Event の publish / subscribe            |
| [R3](https://github.com/Cysharp/R3)          | ReactiveProperty、Observable、ViewSignal |
| [UniTask](https://github.com/cysharp/UniTask)     | 非同期処理、ロード、セーブ、演出待ち                     |
| [PrimeTween](https://github.com/KyryloKuzyk/PrimeTween)  | View 層での Tween / UI 演出                 |

## 主な機能

### Command / TryCommand / ResultCommand

状態を変更する処理を Command として切り出します。

* 成功前提の処理は Command
* 成功 / 失敗だけ返したい処理は TryCommand
* 失敗理由や追加情報を返したい処理は ResultCommand

```csharp
this.SendCommand<AdvanceScenarioCommand>();

var success = this.TrySendCommand<UseItemCommand, int>(itemId);

var result = this.SendResultCommand<
    UseItemWithResultCommand,
    int,
    UseItemResult>(itemId);
```

### Query

状態を読むだけの処理は Query として切り出します。
Query は状態を変更しません。

```csharp
var line = this.SendQuery<GetCurrentLineQuery, ScenarioLine>();
```

### Event

GameService や Command から Presenter へ通知したいときに使います。

```csharp
this.PublishEvent(new ScenarioLineChangedEvent(line));

this.SubscribeEvent<ScenarioLineChangedEvent>(OnScenarioLineChanged);
```

### ViewSignal

View から Presenter へ、入力や UI 操作を伝えるための仕組みです。

ボタン、トグル、選択肢、ドラッグ、アニメーション完了などを、View の外へ安全に公開します。

```csharp
public interface IDialogueView : IView
{
    ViewSignal AdvanceClicked { get; }
    ViewSignal<int> ChoiceSelected { get; }
}
```

Presenter は ViewSignal を購読し、必要な Command を送ります。

```csharp
this.SubscribeViewSignal(
    _view.AdvanceClicked,
    () => this.SendCommand<AdvanceScenarioCommand>());
```

### ReadOnlyModel 自動生成

Presenter には Model 本体ではなく、読み取り専用 interface を渡します。

Model を `partial class` として定義すると、Editor 側で `IReadOnlyXxxModel` を生成します。

これにより、Presenter から Model を直接書き換える経路を減らします。

### Entity

敵、弾、ノーツ、NPC のように実行中に複数生成されるものは Entity として扱えます。

基本構成は次の4つです。

* EntityCollectionModel
* EntityView
* EntityViewSpawner
* EntityPresenter

Model に Entity が追加・更新・削除されると、Presenter が View と同期します。
Prefab 生成、Scene 配置 View、破棄、Pool、Keep などの扱いも EntityViewSpawner 側で管理します。

## Feature.ADV

Reature.ADV は、ADV / ノベルゲーム向けのシナリオ再生機能です。

runtime は Unity UI や特定のファイル形式に固定せず、`AdvScenario` と `IAdvInstruction` を中心に動きます。

標準で扱うもの:

* 会話行
* 選択肢
* ジャンプ
* 条件分岐
* 変数
* 待機
* シグナル
* セーブ / ロード
* バックログ
* 既読 marker

UI は View interface で差し替える前提です。
完成済み UI prefab を押し付けるのではなく、各ゲームの画面設計に合わせて組み込みます。

## Feature.Rhythm

Reature.Rhythm は、レーン型リズムゲーム向けの runtime 機能です。

譜面エディタや完成済み UI は含まず、次のような中核処理を担当します。

* 譜面読み込み
* DSP time ベースの再生管理
* ノーツ表示用データ
* 入力判定
* スコア / コンボ管理
* 判定 Event

ノーツ表示は Entity の仕組みと組み合わせて扱えます。
見た目、入力方式、AudioSource 構成はゲーム側で差し替える前提です。

## 開発状況

現時点では、次のような内容を含んでいます。

* Core architecture
* VContainer integration
* MessagePipe event integration
* R3 based ViewSignal
* ReadOnlyModel generator
* Entity system
* ToolADV
* ToolRhythm

まだ開発途中のため、実運用で使う場合は API 変更を前提にしてください。

## 参考

MyArchitecture は、QFramework の考え方を参考にしています。

[QFramework](https://github.com/seikasan/QFramework)
