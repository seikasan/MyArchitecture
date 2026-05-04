# MyArchitecture

MyArchitecture は、Unity 向けの軽量アーキテクチャです。

View / Presenter / GameService / Model / Utility と分け、状態変更は Command、状態取得は Query、通知は Event、View から Presenter への入力は ViewSignal として扱います。

QFramework の層構造や Command / Query の考え方を参考にしつつ、Unity 開発で扱いやすい形に整理しています。

現在は開発途中です。
API、命名、ディレクトリ構成は変更する可能性があります。

Unity6.3 以降で動作確認済みですが、古くても使える可能性はあります。知らんけど。

## 実装者向けの使い方

MyArchitecture を使って実装するときに、普段呼び出すメソッドを次の文書にまとめています。

[実装者向けの使い方を見る](Documentation~/usage.md)

## 作った背景

MyArchitecture は、QFramework や VContainer を使ったときに感じた課題を、自分なりに解決するために作ったものです。

QFramework では、IController は Model を参照するだけで、変更は Command や System 経由で行う考え方があります。
ただ、実装上は Model を取得して直接変更できてしまう場合があります。

チームに初めて QFramework を触る人がいる場合、規約として書いてあっても、機能上できてしまうことは事故につながります。
そのため MyArchitecture では、Presenter には Model 本体ではなく、読み取り専用 interface を渡す方針にしています。

また、pure C# の event や Observable の購読は、破棄を忘れるとバグの原因になります。
MyArchitecture では、購読を自動でライフタイムに紐づける API を用意し、購読破棄忘れを減らします。

Unity では、View と Presenter を分けた方が扱いやすいと私は思っています。
View は表示、入力、演出を担当し、Presenter は View とアプリケーション側をつなぎます。

QFramework の System 層は、C# の System 名前空間と被るため、このアーキテクチャでは GameService と呼びます。

Command は毎回 new するのではなく、DI で解決して実行する前提です。
VContainer と組み合わせやすくし、Command / Query / GameService / Model / Presenter を同じ DI の仕組みに乗せます。

さらに、敵、弾、ノーツ、NPC のように実行中に複数生成されるデータを扱うため、Entity の仕組みを用意しています。

## 基本の流れ

```text
View input
→ ViewSignal
→ Presenter
→ Command
→ GameService / Model

Model change / Event
→ Presenter
→ View update
```

## 層の役割

| 層           | 役割                                             |
| ----------- | ---------------------------------------------- |
| View        | Unity の表示、入力、演出を扱う                             |
| Presenter   | View、ReadOnlyModel、Event を見て、Command / Query を送る |
| GameService | ゲームルールや Model の変更を担当する                         |
| Model       | ゲーム状態を持つ                                       |
| Utility     | 計算、変換、保存、読み込み、外部 API のラップなどを担当する               |
| Command     | 状態変更を表す処理                                      |
| Query       | 状態取得や計算結果の取得を表す処理                              |
| Event       | 下位層から Presenter への通知                           |
| ViewSignal  | View から Presenter への通知                         |

## ルール

* View は Presenter を直接参照しない
* View は入力や UI 操作を ViewSignal として公開する
* Presenter は GameService を直接参照しない
* Presenter から状態を変えるときは Command を送る
* Presenter が状態を読むときは ReadOnlyModel または Query を使う
* Model は View / Presenter / GameService を知らない
* Model の書き換えは GameService または Command 経由にする
* 下位層から Presenter への通知は Event を使う
* Event は命令ではなく、起きたことの通知として扱う
* ViewSignal / Observable の購読は SubscribeTo を使う
* Event の購読は SubscribeEvent を使う
* View の Tween や Animation は View 層に閉じ込める

## Command

### Command

状態を変更する処理は Command として実装します。

```csharp
this.SendCommand<AdvanceScenarioCommand>();
```

引数がある場合は、Command 型と引数型を指定します。

```csharp
this.SendCommand<SelectChoiceCommand, int>(choiceIndex);
```

複数の値を渡したい場合は、タプルまたは専用の構造体を使います。

```csharp
this.SendCommand<MoveItemCommand, (int from, int to)>((from, to));
```

### TryCommand

成功 / 失敗だけ返したい場合は TryCommand を使います。

```csharp
var success = this.TrySendCommand<UseItemCommand, int>(itemId);
```

### ResultCommand

結果を返したい場合は ResultCommand を使います。

```csharp
var result = this.SendResultCommand<
    UseItemCommand,
    int,
    UseItemResult>(itemId);
```

### Async 系 Command

非同期の場合は Async 系を使います。

```csharp
await this.SendCommandAsync<LoadScenarioCommand>();
var success = await this.TrySendCommandAsync<UseItemCommand, int>(itemId);
```

## Query

状態を読むだけの処理は Query として実装します。
Query は状態を変更しません。

```csharp
var line = this.SendQuery<GetCurrentLineQuery, ScenarioLine>();
```

引数がある場合も Command と同じように扱います。

```csharp
var item = this.SendQuery<GetItemQuery, int, ItemData>(itemId);
```

複数の値を渡す場合は、タプルまたは構造体を使います。

```csharp
var distance = this.SendQuery<
    GetDistanceQuery,
    (int fromId, int toId),
    float>((fromId, toId));
```

## ViewSignal / Observable(ReactiveProperty) の購読

ViewSignal や R3 の Observable は SubscribeTo で購読します。

```csharp
this.SubscribeTo(
    _view.AdvanceClicked,
    () => this.SendCommand<AdvanceScenarioCommand>());
```

値付きの ViewSignal も同じです。

```csharp
this.SubscribeTo(
    _view.ChoiceSelected,
    choiceIndex => this.SendCommand<SelectChoiceCommand, int>(choiceIndex));
```

Observable も SubscribeTo で購読します。

```csharp
this.SubscribeTo(
    _model.CurrentHp,
    hp => _view.SetHp(hp));
```

非同期処理を行う場合は SubscribeToAsync を使います。

```csharp
this.SubscribeToAsync(
    _model.CurrentLine,
    async (line, ct) =>
    {
        await _view.PlayLineAsync(line, ct);
    });
```

連続入力を抑えたい場合は SubscribeToThrottled を使います。

```csharp
this.SubscribeToThrottled(
    _view.AdvanceClicked,
    () => this.SendCommand<AdvanceScenarioCommand>(),
    TimeSpan.FromMilliseconds(300));
```

## Event

GameService や Command から Presenter へ通知したい場合は Event を使います。

```csharp
this.PublishEvent(new ScenarioLineChangedEvent(line));
```

Event は命令ではなく、すでに起きたことの通知として扱います。

```csharp
ScenarioLineChangedEvent
ItemUsedEvent
NoteJudgedEvent
```

Event は SubscribeEvent で購読します。

```csharp
this.SubscribeEvent<ScenarioLineChangedEvent>(eventData =>
{
    _view.SetLine(eventData.Line);
});
```

非同期処理を行う場合は、CancellationToken を受け取る overload を使います。

```csharp
this.SubscribeEvent<ScenarioLineChangedEvent>(async (eventData, ct) =>
{
    await _view.PlayLineAsync(eventData.Line, ct);
});
```

## ReadOnlyModel

Presenter には Model 本体ではなく、自動生成された読み取り専用 interface を渡します。

これにより、Presenter から Model を直接書き換える経路を遮断します。

```csharp
private readonly IReadOnlyBoardModel _model;

public BoardPresenter(IReadOnlyBoardModel model)
{
    _model = model;
}
```

状態を読むだけなら ReadOnlyModel または Query を使います。

## Entity

Entity については、まだまだ改善の余地があるように思います。

敵、弾、ノーツ、NPC のように実行中に複数生成されるものは Entity として扱います。

基本構成は次の 4 つです。

* EntityCollectionModel
* EntityView
* EntityViewSpawner
* EntityPresenter

Model に Entity が追加、更新、削除されると、Presenter が View と同期します。

Prefab 生成、Scene 配置済み View、破棄、Pool、Keep などの扱いは EntityViewSpawner 側で管理します。

## 使用ライブラリ

| ライブラリ       | 用途                                     |
| ----------- | -------------------------------------- |
| [VContainer](https://github.com/hadashiA/VContainer)  | DI、Lifetime 管理、Command / Query の組み立て   |
| [MessagePipe](https://github.com/Cysharp/MessagePipe) | Event の publish / subscribe            |
| [R3](https://github.com/Cysharp/R3)          | ReactiveProperty、Observable、ViewSignal |
| [UniTask](https://github.com/cysharp/UniTask)     | 非同期処理、ロード、セーブ、演出待ち                     |

## 任意ライブラリ

| ライブラリ | 用途 |
| --- | --- |
| [PrimeTween](https://github.com/KyryloKuzyk/PrimeTween) | View 層での Tween / UI 演出 |

PrimeTween 統合を使う場合は、PrimeTween を導入してください。

## 参考

MyArchitecture は、QFramework の考え方を参考にしています。

[QFramework](https://github.com/liangxiegame/QFramework)
