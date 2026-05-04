# MyArchitecture 実装者向けの使い方

この文書では、MyArchitecture を使って実装するときに普段使うメソッドと、各層の書き方をまとめます。

設計の考え方や層の役割は README を参照してください。
ここでは、実装中に迷いやすい次の内容を扱います。

* どの層でどのメソッドを使うか
* `ArchitectureLifetimeScope` の書き方
* 自動登録で何がDIに登録されるか
* ViewSignal / Command / Query / Event の使い方
* ReadOnlyModel の自動生成

## 層ごとに使うもの

| 層             | 継承または実装                                                                          | 主に使うもの                                                   | 備考                                                  |
| ------------- | -------------------------------------------------------------------------------- | -------------------------------------------------------- | --------------------------------------------------- |
| View          | `View` を継承し、必要に応じて `IView` 派生interfaceを実装                                        | `ViewSignal.Publish`、View更新用メソッド                         | Unityの入力、表示、演出を扱う。CommandやQueryは送らない                |
| Presenter     | `Presenter`                                                                      | `SendCommand`、`SendQuery`、`SubscribeEvent`、`SubscribeTo` | Viewとアプリケーション側をつなぐ。ViewSignalやModelのObservableを購読する |
| GameService   | `GameService`                                                                    | `SendQuery`、`PublishEvent`                               | ゲームルールやModel変更を扱う。Presenterへ通知したいときはEventを発行する      |
| Command       | `Command` / `Command<TArg>` / `TryCommand` / `ResultCommand` / `AsyncCommand` など | `SendQuery`、`SendCommand`、`PublishEvent`                 | 状態変更を表す処理。必要に応じてModelやGameServiceをコンストラクタで受け取る      |
| Query         | `Query<TResult>` / `Query<TArg, TResult>` / `AsyncQuery<TResult>` など             | `SendQuery`                                              | 状態取得や計算結果の取得を表す処理。状態は変更しない                          |
| Model         | `Model`                                                                          | 公開プロパティ、変更用メソッド                                          | 状態を持つ。Presenterには自動生成されたReadOnlyModelを渡す            |
| Utility       | `Utility` または `IUtility`                                                         | 通常のメソッド                                                  | 計算、変換、保存、読み込み、外部APIのラップなどを担当する                      |
| Event         | `IEvent`                                                                         | データ定義                                                    | 下位層からPresenterへの通知として使う                             |
| LifetimeScope | `ArchitectureLifetimeScope`                                                      | `RegisterModels` など                                      | DI登録とMyArchitectureの初期化を行う                          |

## 使えるメソッド早見表

| メソッド                     | Presenter | GameService | Command | Query | View | Model | Utility |
| ------------------------ | --------- | ----------- |---|---| - |---| - |
| `SendCommand`            | ○         | ×           | ○ | × | × | × | × |
| `TrySendCommand`         | ○         | ×           | ○ | × | × | × | × |
| `SendResultCommand`      | ○         | ×           | ○ | × | × | × | × |
| `SendCommandAsync`       | ○         | ×           | ○ | × | × | × | × |
| `TrySendCommandAsync`    | ○         | ×           | ○ | × | × | × | × |
| `SendResultCommandAsync` | ○         | ×           | ○ | × | × | × | × |
| `SendQuery`              | ○         | ○           | ○ | ○ | × | × | × |
| `SendQueryAsync`         | ○         | ○           | ○ | ○ | × | × | × |
| `PublishEvent`           | ×         | ○           | ○ | × | × | × | × |
| `SubscribeEvent`         | ○         | ×           | × | × | × | × | × |
| `SubscribeTo`            | ○         | 必要な場合のみ     | × | × | × | × | × |
| `SubscribeToAsync`       | ○         | 必要な場合のみ     | × | × | × | × | × |
| `SubscribeToThrottled`   | ○         | 必要な場合のみ     | × | × | × | × | × |

`SubscribeTo` 系は `IHasArchitectureLifetime` を持つ型で使えます。
普段はPresenterでViewSignalやObservableを購読する用途が中心です。

## 初期化の流れ

`Presenter`、`GameService`、`Model`、Command系、Query系は `ArchitectureObject` を基底に持っています。

初期化では次の順に呼ばれます。

| メソッド               | 書く内容                                     |
| ------------------ | ---------------------------------------- |
| `OnInitialize`     | 自分の内部状態だけを初期化する                          |
| `OnBind`           | 他オブジェクトのViewSignal、Observable、Eventを購読する |
| `OnPostInitialize` | 全員のBind後に、初回同期や初回Commandを送る              |
| `OnDispose`        | 明示的に破棄したいものを片付ける                         |

購読は `Track` または `SubscribeTo` / `SubscribeEvent` 経由で登録すると、破棄時にまとめてDisposeされます。

Viewは `MonoBehaviour` を継承した `View` を基底に持っています。
View側では `OnAwake`、`OnStart`、`OnViewDestroy` を必要に応じて上書きします。

## LifetimeScope の書き方

シーンには、VContainerの `LifetimeScope` として `ArchitectureLifetimeScope` を継承したクラスを置きます。

手動登録の必要がない場合は、`DefaultArchitectureLifetimeScope` をそのまま使います。

`Configure` はMyArchitecture側で使っているため、直接上書きしません。
必要な登録は、用意されているメソッドを上書きして書きます。

```csharp
using MyArchitecture.Integration;
using VContainer;

public sealed class GameLifetimeScope : ArchitectureLifetimeScope
{
    protected override void RegisterViews(
        ArchitectureRegistrationContext context)
    {
        // Scene上のViewは自動登録されるため、
        // 手動で渡したいViewだけを書く
    }

    protected override void RegisterModels(
        ArchitectureRegistrationContext context)
    {
        context.RegisterModel<PlayerModel>();
    }

    protected override void RegisterGameServices(
        ArchitectureRegistrationContext context)
    {
        context.RegisterGameService<PlayerGameService>();
    }

    protected override void RegisterPresenters(
        ArchitectureRegistrationContext context)
    {
        context.RegisterPresenter<PlayerPresenter>();
    }

    protected override void RegisterCommands(
        ArchitectureRegistrationContext context)
    {
        context.RegisterCommand<DamagePlayerCommand>();
    }

    protected override void RegisterQueries(
        ArchitectureRegistrationContext context)
    {
        context.RegisterQuery<GetPlayerHpQuery>();
    }

    protected override void RegisterEvents(
        ArchitectureRegistrationContext context)
    {
        context.RegisterEvent<PlayerHpChangedEvent>();
    }

    protected override void RegisterUtilities(
        ArchitectureRegistrationContext context)
    {
        context.RegisterUtility<DamageCalculator>();
    }
}
```

## LifetimeScope で行われる登録

`ArchitectureLifetimeScope` では、次の登録が行われます。

| 登録内容                      | 説明                                             |
| ------------------------- | ---------------------------------------------- |
| `ArchitectureSettings`    | MyArchitectureの設定                              |
| `CommandRegistry`         | Command実行に使う登録情報                               |
| `QueryRegistry`           | Query実行に使う登録情報                                 |
| `ICommandRunner`          | Commandを送るための実行役                               |
| `IQueryRunner`            | Queryを送るための実行役                                 |
| `IEventPublisher`         | Event発行用                                       |
| `IEventSubscriber`        | Event購読用                                       |
| MessagePipe               | Eventのpublish / subscribeに使う                   |
| Scene上のView               | Sceneに配置されている `View` / `IView` を自動で登録する        |
| Package側の自動登録             | `ArchitecturePackageAutoRegistration.g.cs` を呼ぶ |
| Project側の自動登録             | `ArchitectureAutoRegistration.g.cs` を探して呼ぶ     |
| Project側の手動まとめ登録          | `RegisterProjectAutoRegistration` を呼ぶ          |
| `ArchitectureInitializer` | 初期化対象のInitialize / Bind / PostInitializeを呼ぶ    |

## 自動登録の対象

Runtime側に置いた対象クラスは、自動生成された `ArchitectureAutoRegistration.g.cs` に登録コードが出力されます。

ただし、次の型は対象外です。

* `Editor` 配下のスクリプト
* `Generated` 配下のスクリプト
* `Tests` 配下のスクリプト
* `abstract` class
* open generic
* nested type
* interface
* enum

自動登録の対象は次の型です。

| 種類          | 自動登録の対象                                              |
| ----------- | ---------------------------------------------------- |
| Model       | `Model` を継承した具象クラス                                   |
| GameService | `GameService` を継承した具象クラス                             |
| Presenter   | `Presenter` を継承した具象クラス                               |
| Command     | `ICommand` 系を実装した具象クラス。通常は `Command` 系を継承する          |
| Query       | `IQuery` 系を実装した具象クラス。通常は `Query` 系を継承する              |
| Utility     | `IUtility` を実装した具象クラス                                |
| Event       | `IEvent` を実装した型                                      |
| EntityView  | `IEntityView<T>` または `ISceneEntityView<T>` を実装したView |

## 自動登録でDIに登録される型

自動登録は、単にクラスを見つけるだけではありません。
見つけた型を、どの型としてDIから解決できるようにするかが種類ごとに違います。

| 種類          | DIで解決できる型                                                          | 補足                                                        |
| ----------- |--------------------------------------------------------------------| --------------------------------------------------------- |
| Model       | 具象型、生成されたReadOnlyModel interface、`IArchitectureInitializable`      | `IModel` としては登録されない                                       |
| GameService | 具象型、`IGameService`、`IArchitectureInitializable`                    | 個別interfaceを付けても、自動ではそのinterfaceには登録されない                  |
| Presenter   | 具象型、`IPresenter`、`IArchitectureInitializable`                      | 個別interfaceを付けても、自動ではそのinterfaceには登録されない                  |
| Command     | 具象型                                                                | 実行時は `CommandRegistry` 経由で解決される                           |
| Query       | 具象型                                                                | 実行時は `QueryRegistry` 経由で解決される                             |
| Utility     | 具象型、実装しているinterface                                                | `AsImplementedInterfaces` で登録される                          |
| Event       | MessagePipeのBroker                                                 | 通常のDI解決用ではなく、publish / subscribe用に登録される                   |
| Scene View  | 具象型、`IView` を継承したView interface                                    | `IView` 自体、`ICanExposeViewSignal`、`ICanUseTween` には登録されない |
| EntityView  | `EntityViewRegistry`、`EntityViewFactory`、`PooledEntityViewFactory` | 通常のScene Viewとは扱いが分かれる                                    |

## 具象型とinterface登録の考え方

### Model

Modelは、具象型と生成されたReadOnlyModel interfaceとして登録されます。

```csharp
public partial class PlayerModel : Model
{
    public int Hp { get; private set; }

    public void SetHp(int hp)
    {
        Hp = hp;
    }
}
```

この場合、ReadOnlyModel生成後は次のように使えます。

```csharp
public sealed class PlayerGameService : GameService
{
    private readonly PlayerModel _model;

    public PlayerGameService(PlayerModel model)
    {
        _model = model;
    }
}
```

```csharp
public sealed class PlayerPresenter : Presenter
{
    private readonly IReadOnlyPlayerModel _model;

    public PlayerPresenter(IReadOnlyPlayerModel model)
    {
        _model = model;
    }
}
```

Presenterには `PlayerModel` 本体ではなく、`IReadOnlyPlayerModel` を渡します。
状態を変更したい場合はCommandを送ります。

```csharp
this.SendCommand<DamagePlayerCommand, int>(10);
```

### GameService

GameServiceは、具象型、`IGameService`、`IArchitectureInitializable` として登録されます。

```csharp
public sealed class PlayerGameService : GameService
{
}
```

Commandなどから特定のServiceを使う場合は、具象型で受け取ります。

```csharp
public sealed class DamagePlayerCommand : Command<int>
{
    private readonly PlayerGameService _service;

    public DamagePlayerCommand(PlayerGameService service)
    {
        _service = service;
    }

    protected override void OnExecute(int damage)
    {
        _service.Damage(damage);
    }
}
```

独自interface、たとえば `IPlayerGameService` として解決したい場合は、自動登録だけでは足りません。
その場合は手動登録を追加します。

```csharp
protected override void RegisterGameServices(
    ArchitectureRegistrationContext context)
{
    context.Builder
        .Register<PlayerGameService>(context.DefaultLifetime)
        .AsSelf()
        .As<IPlayerGameService>()
        .As<IGameService>()
        .As<IArchitectureInitializable>();
}
```

### Presenter

Presenterは、具象型、`IPresenter`、`IArchitectureInitializable` として登録されます。

```csharp
public sealed class PlayerPresenter : Presenter
{
}
```

Presenterを個別interfaceでInjectする想定は基本にしません。
MyArchitecture側が `IPresenter` としてまとめて初期化します。

### Command / Query

CommandとQueryは具象型として登録されます。

```csharp
context.RegisterCommand<DamagePlayerCommand>();
context.RegisterQuery<GetPlayerHpQuery>();
```

呼び出し側はDIから直接CommandやQueryを受け取るのではなく、`SendCommand` / `SendQuery` を使います。

```csharp
this.SendCommand<DamagePlayerCommand, int>(10);

var hp = this.SendQuery<GetPlayerHpQuery, int>();
```

CommandやQueryの中で別のCommandやQueryを使いたい場合は、Command / Query自身が持っている `SendCommand` / `SendQuery` を使います。

### Utility

Utilityは、具象型と、実装しているinterfaceで登録されます。

```csharp
public interface IDamageCalculator
{
    int Calculate(int baseDamage);
}

public sealed class DamageCalculator :
    Utility,
    IDamageCalculator
{
    public int Calculate(int baseDamage)
    {
        return baseDamage;
    }
}
```

この場合、次のようにinterfaceで受け取れます。

```csharp
public sealed class DamagePlayerCommand : Command<int>
{
    private readonly IDamageCalculator _calculator;

    public DamagePlayerCommand(IDamageCalculator calculator)
    {
        _calculator = calculator;
    }

    protected override void OnExecute(int damage)
    {
        var actualDamage = _calculator.Calculate(damage);
    }
}
```

### View

Viewは `MyArchitecture.Unity.View` を継承します。
Presenterから受け取るためのinterfaceを分けたい場合は、`IView` を継承したView interfaceを作ります。

```csharp
using MyArchitecture.Core;

public interface IPlayerView : IView
{
    ViewSignal DamageClicked { get; }

    void SetHp(int hp);
}
```

```csharp
using MyArchitecture.Core;
using MyArchitecture.Unity;

public sealed class PlayerView :
    View,
    IPlayerView
{
    public ViewSignal DamageClicked { get; } = new();

    protected override void OnAwake()
    {
        // Unityの入力イベントから DamageClicked.Publish() を呼ぶ
    }

    public void SetHp(int hp)
    {
        // HP表示を更新する
    }
}
```

この場合、Presenterでは `IPlayerView` として受け取れます。

```csharp
public sealed class PlayerPresenter : Presenter
{
    private readonly IPlayerView _view;

    public PlayerPresenter(IPlayerView view)
    {
        _view = view;
    }
}
```

同じView interfaceを実装したScene Viewが複数ある場合は重複エラーになります。
複数存在するViewを扱いたい場合は、通常のScene ViewではなくEntityViewとして扱います。

## Model と ReadOnlyModel の自動生成

Modelは `partial class` にします。

```csharp
using MyArchitecture.Core;
using R3;

public partial class PlayerModel : Model
{
    public int Hp { get; private set; } = 100;

    public readonly ReactiveProperty<int> Level = new(1);

    public void SetHp(int hp)
    {
        Hp = hp;
    }
}
```

このModelから、読み取り専用interfaceが自動生成されます。

```csharp
public interface IReadOnlyPlayerModel : IReadOnlyModel
{
    int Hp { get; }

    ReadOnlyReactiveProperty<int> Level { get; }
}
```

生成されたinterfaceは、PresenterにInjectして使います。

```csharp
using MyArchitecture.Core;
using MyArchitecture.Integration;

public sealed class PlayerPresenter : Presenter
{
    private readonly IPlayerView _view;
    private readonly IReadOnlyPlayerModel _model;

    public PlayerPresenter(
        IPlayerView view,
        IReadOnlyPlayerModel model)
    {
        _view = view;
        _model = model;
    }

    protected override void OnBind()
    {
        this.SubscribeTo(
            _model.Level,
            level => _view.SetLevel(level));
    }

    protected override void OnPostInitialize()
    {
        _view.SetHp(_model.Hp);
    }
}
```

Model本体はPresenterにInjectしません。
Presenterから状態を変更したい場合はCommandを使います。

```csharp
this.SendCommand<DamagePlayerCommand, int>(10);
```

## ReadOnlyModel が生成される条件

ReadOnlyModelは次の条件を満たすModelから生成されます。

| 条件                       | 内容                                               |
| ------------------------ | ------------------------------------------------ |
| `Model` を継承している          | `public partial class PlayerModel : Model` の形にする |
| 具象クラスである                 | `abstract` やopen genericは対象外                     |
| `partial` が付いている         | 生成されたpartial定義と結合するため                            |
| Runtime側のScriptである       | `Editor` や `Generated` 配下は対象外                    |
| 公開getterがある              | `public int Hp { get; private set; }` は対象になる     |
| public readonly fieldである | `public readonly SomeType Value` は対象になる          |

`ReactiveProperty<T>` のfieldは、ReadOnly側では `ReadOnlyReactiveProperty<T>` として公開されます。
それ以外のpublic readonly fieldは、その型のままReadOnly側に公開されます。

`partial` を忘れると、ReadOnlyModelは生成されません。
その場合、Model登録時に読み取り専用interfaceが見つからずエラーになります。

## View の書き方

ViewはUnityの入力、表示、演出を扱います。

ViewはCommandを送りません。
入力は `ViewSignal` として公開し、Presenterが購読します。

```csharp
using MyArchitecture.Core;

public interface IPlayerView : IView
{
    ViewSignal DamageClicked { get; }

    void SetHp(int hp);
    void SetLevel(int level);
}
```

```csharp
using MyArchitecture.Core;
using MyArchitecture.Unity;
using UnityEngine;
using UnityEngine.UI;

public sealed class PlayerView :
    View,
    IPlayerView
{
    [SerializeField]
    private Button damageButton;

    public ViewSignal DamageClicked { get; } = new();

    protected override void OnAwake()
    {
        damageButton.onClick.AddListener(
            () => DamageClicked.Publish());
    }

    protected override void OnViewDestroy()
    {
        damageButton.onClick.RemoveAllListeners();
        DamageClicked.Dispose();
    }

    public void SetHp(int hp)
    {
        // HP表示を更新する
    }

    public void SetLevel(int level)
    {
        // レベル表示を更新する
    }
}
```

値を渡す場合は `ViewSignal<T>` を使います。

```csharp
public interface IInventoryView : IView
{
    ViewSignal<int> ItemSelected { get; }

    void SetItems(IReadOnlyList<ItemViewData> items);
}
```

## Presenter の書き方

Presenterは、View、ReadOnlyModel、Eventを見て、CommandやQueryを送ります。

```csharp
using System;
using MyArchitecture.Core;
using MyArchitecture.Integration;

public sealed class PlayerPresenter : Presenter
{
    private readonly IPlayerView _view;
    private readonly IReadOnlyPlayerModel _model;

    public PlayerPresenter(
        IPlayerView view,
        IReadOnlyPlayerModel model)
    {
        _view = view;
        _model = model;
    }

    protected override void OnBind()
    {
        this.SubscribeTo(
            _view.DamageClicked,
            () => this.SendCommand<DamagePlayerCommand, int>(10));

        this.SubscribeTo(
            _model.Level,
            level => _view.SetLevel(level));

        this.SubscribeEvent<PlayerHpChangedEvent>(
            eventData => _view.SetHp(eventData.Hp));
    }

    protected override void OnPostInitialize()
    {
        _view.SetHp(_model.Hp);
    }
}
```

連続入力を抑えたい場合は `SubscribeToThrottled` を使います。

```csharp
this.SubscribeToThrottled(
    _view.DamageClicked,
    () => this.SendCommand<DamagePlayerCommand, int>(10),
    TimeSpan.FromMilliseconds(300));
```

非同期処理を含む購読では `SubscribeToAsync` を使います。

```csharp
this.SubscribeToAsync(
    _model.Level,
    async (level, ct) =>
    {
        await _view.PlayLevelUpAsync(level, ct);
    });
```

## GameService の書き方

GameServiceは、ゲームルールやModel変更を担当します。

Presenterへ通知したい場合はEventを発行します。

```csharp
using System;
using MyArchitecture.Core;

public sealed class PlayerGameService : GameService
{
    private readonly PlayerModel _model;

    public PlayerGameService(PlayerModel model)
    {
        _model = model;
    }

    public void Damage(int value)
    {
        var nextHp = Math.Max(0, _model.Hp - value);

        _model.SetHp(nextHp);

        this.PublishEvent(new PlayerHpChangedEvent(nextHp));
    }
}
```

GameServiceから状態を読むだけならQueryを使えます。

```csharp
var canDamage = this.SendQuery<CanDamagePlayerQuery, bool>();
```

GameServiceはCommandを送りません。
状態変更を連鎖させたい場合は、CommandではなくGameServiceのメソッドやModel変更として整理します。

## Command の書き方

状態変更はCommandとして実装します。

実装者は `CommandBase` ではなく、用途に合った `Command` 系のクラスを継承します。

### 引数なしのCommand

```csharp
using MyArchitecture.Core;

public sealed class HealPlayerCommand : Command
{
    private readonly PlayerGameService _service;

    public HealPlayerCommand(PlayerGameService service)
    {
        _service = service;
    }

    protected override void OnExecute()
    {
        _service.Heal(10);
    }
}
```

呼び出し側は次のように書きます。

```csharp
this.SendCommand<HealPlayerCommand>();
```

### 引数ありのCommand

引数がある場合は `Command<TArg>` を継承します。

```csharp
using MyArchitecture.Core;

public sealed class DamagePlayerCommand : Command<int>
{
    private readonly PlayerGameService _service;

    public DamagePlayerCommand(PlayerGameService service)
    {
        _service = service;
    }

    protected override void OnExecute(int damage)
    {
        _service.Damage(damage);
    }
}
```

呼び出し側は次のように書きます。

```csharp
this.SendCommand<DamagePlayerCommand, int>(10);
```

複数の値を渡したい場合は、タプルまたは専用の型を使います。

```csharp
this.SendCommand<MoveItemCommand, (int from, int to)>((from, to));
```

### 成功 / 失敗を返すCommand

成功 / 失敗だけ返したい場合は `TryCommand` または `TryCommand<TArg>` を継承します。

```csharp
using MyArchitecture.Core;

public sealed class UseItemCommand : TryCommand<int>
{
    private readonly InventoryService _service;

    public UseItemCommand(InventoryService service)
    {
        _service = service;
    }

    protected override bool OnTryExecute(int itemId)
    {
        return _service.TryUse(itemId);
    }
}
```

呼び出し側は次のように書きます。

```csharp
var success =
    this.TrySendCommand<UseItemCommand, int>(itemId);
```

### 結果を返すCommand

結果オブジェクトを返したい場合は `ResultCommand<TResult>` または `ResultCommand<TArg, TResult>` を継承します。

`TResult` は `ICommandResult` を実装している必要があります。

```csharp
using MyArchitecture.Core;

public readonly struct UseItemResult : ICommandResult
{
    public UseItemResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    public bool Success { get; }
    public string Message { get; }
}
```

```csharp
using MyArchitecture.Core;

public sealed class UseItemCommand :
    ResultCommand<int, UseItemResult>
{
    private readonly InventoryService _service;

    public UseItemCommand(InventoryService service)
    {
        _service = service;
    }

    protected override UseItemResult OnExecute(int itemId)
    {
        return _service.Use(itemId);
    }
}
```

呼び出し側は次のように書きます。

```csharp
var result =
    this.SendResultCommand<
        UseItemCommand,
        int,
        UseItemResult>(itemId);
```

### 非同期Command

非同期処理を行う場合は `AsyncCommand` または `AsyncCommand<TArg>` を継承します。

```csharp
using System.Threading;
using Cysharp.Threading.Tasks;
using MyArchitecture.Core;

public sealed class LoadPlayerDataCommand : AsyncCommand
{
    private readonly PlayerDataService _service;

    public LoadPlayerDataCommand(PlayerDataService service)
    {
        _service = service;
    }

    protected override async UniTask OnExecuteAsync(
        CancellationToken cancellationToken)
    {
        await _service.LoadAsync(cancellationToken);
    }
}
```

呼び出し側は次のように書きます。

```csharp
await this.SendCommandAsync<LoadPlayerDataCommand>();
```

引数がある場合は `AsyncCommand<TArg>` を継承します。

```csharp
using System.Threading;
using Cysharp.Threading.Tasks;
using MyArchitecture.Core;

public sealed class LoadScenarioCommand : AsyncCommand<string>
{
    private readonly ScenarioService _service;

    public LoadScenarioCommand(ScenarioService service)
    {
        _service = service;
    }

    protected override async UniTask OnExecuteAsync(
        string scenarioId,
        CancellationToken cancellationToken)
    {
        await _service.LoadAsync(scenarioId, cancellationToken);
    }
}
```

```csharp
await this.SendCommandAsync<LoadScenarioCommand, string>(scenarioId);
```

## Command の種類

| 継承するクラス                             | 呼び出しメソッド                                               | 実装するメソッド                                            | 用途                   |
| ----------------------------------- | ------------------------------------------------------ | --------------------------------------------------- | -------------------- |
| `Command`                           | `SendCommand<TCommand>()`                              | `OnExecute()`                                       | 引数なしで状態を変更する         |
| `Command<TArg>`                     | `SendCommand<TCommand, TArg>(arg)`                     | `OnExecute(TArg arg)`                               | 引数ありで状態を変更する         |
| `TryCommand`                        | `TrySendCommand<TCommand>()`                           | `OnTryExecute()`                                    | 成功 / 失敗を返す           |
| `TryCommand<TArg>`                  | `TrySendCommand<TCommand, TArg>(arg)`                  | `OnTryExecute(TArg arg)`                            | 引数ありで成功 / 失敗を返す      |
| `ResultCommand<TResult>`            | `SendResultCommand<TCommand, TResult>()`               | `OnExecute()`                                       | 結果オブジェクトを返す          |
| `ResultCommand<TArg, TResult>`      | `SendResultCommand<TCommand, TArg, TResult>(arg)`      | `OnExecute(TArg arg)`                               | 引数ありで結果オブジェクトを返す     |
| `AsyncCommand`                      | `SendCommandAsync<TCommand>()`                         | `OnExecuteAsync(CancellationToken ct)`              | 非同期で状態を変更する          |
| `AsyncCommand<TArg>`                | `SendCommandAsync<TCommand, TArg>(arg)`                | `OnExecuteAsync(TArg arg, CancellationToken ct)`    | 引数ありの非同期Command      |
| `AsyncTryCommand`                   | `TrySendCommandAsync<TCommand>()`                      | `OnTryExecuteAsync(CancellationToken ct)`           | 非同期で成功 / 失敗を返す       |
| `AsyncTryCommand<TArg>`             | `TrySendCommandAsync<TCommand, TArg>(arg)`             | `OnTryExecuteAsync(TArg arg, CancellationToken ct)` | 引数ありで非同期の成功 / 失敗を返す  |
| `AsyncResultCommand<TResult>`       | `SendResultCommandAsync<TCommand, TResult>()`          | `OnExecuteAsync(CancellationToken ct)`              | 非同期で結果オブジェクトを返す      |
| `AsyncResultCommand<TArg, TResult>` | `SendResultCommandAsync<TCommand, TArg, TResult>(arg)` | `OnExecuteAsync(TArg arg, CancellationToken ct)`    | 引数ありで非同期の結果オブジェクトを返す |

## Query の書き方

Queryは、状態取得や計算結果の取得を表します。
状態は変更しません。

実装者は `QueryBase` ではなく、用途に合った `Query` 系のクラスを継承します。

### 引数なしのQuery

```csharp
using MyArchitecture.Core;

public sealed class GetPlayerHpQuery : Query<int>
{
    private readonly IReadOnlyPlayerModel _model;

    public GetPlayerHpQuery(IReadOnlyPlayerModel model)
    {
        _model = model;
    }

    protected override int OnExecute()
    {
        return _model.Hp;
    }
}
```

呼び出し側は次のように書きます。

```csharp
var hp = this.SendQuery<GetPlayerHpQuery, int>();
```

### 引数ありのQuery

引数がある場合は `Query<TArg, TResult>` を継承します。

```csharp
using MyArchitecture.Core;

public sealed class HasItemQuery : Query<int, bool>
{
    private readonly IReadOnlyInventoryModel _model;

    public HasItemQuery(IReadOnlyInventoryModel model)
    {
        _model = model;
    }

    protected override bool OnExecute(int itemId)
    {
        return _model.HasItem(itemId);
    }
}
```

呼び出し側は次のように書きます。

```csharp
var hasItem =
    this.SendQuery<HasItemQuery, int, bool>(itemId);
```

### 非同期Query

非同期で値を返したい場合は `AsyncQuery<TResult>` または `AsyncQuery<TArg, TResult>` を継承します。

```csharp
using System.Threading;
using Cysharp.Threading.Tasks;
using MyArchitecture.Core;

public sealed class LoadMasterDataQuery : AsyncQuery<MasterData>
{
    private readonly MasterDataRepository _repository;

    public LoadMasterDataQuery(MasterDataRepository repository)
    {
        _repository = repository;
    }

    protected override async UniTask<MasterData> OnExecuteAsync(
        CancellationToken cancellationToken)
    {
        return await _repository.LoadAsync(cancellationToken);
    }
}
```

呼び出し側は次のように書きます。

```csharp
var data =
    await this.SendQueryAsync<LoadMasterDataQuery, MasterData>();
```

引数がある場合は `AsyncQuery<TArg, TResult>` を継承します。

```csharp
using System.Threading;
using Cysharp.Threading.Tasks;
using MyArchitecture.Core;

public sealed class LoadItemDataQuery :
    AsyncQuery<int, ItemData>
{
    private readonly ItemDataRepository _repository;

    public LoadItemDataQuery(ItemDataRepository repository)
    {
        _repository = repository;
    }

    protected override async UniTask<ItemData> OnExecuteAsync(
        int itemId,
        CancellationToken cancellationToken)
    {
        return await _repository.LoadAsync(itemId, cancellationToken);
    }
}
```

```csharp
var item =
    await this.SendQueryAsync<LoadItemDataQuery, int, ItemData>(itemId);
```

## Query の種類

| 継承するクラス                     | 呼び出しメソッド                                     | 実装するメソッド                                         | 用途            |
| --------------------------- | -------------------------------------------- | ------------------------------------------------ | ------------- |
| `Query<TResult>`            | `SendQuery<TQuery, TResult>()`               | `OnExecute()`                                    | 引数なしで値を返す     |
| `Query<TArg, TResult>`      | `SendQuery<TQuery, TArg, TResult>(arg)`      | `OnExecute(TArg arg)`                            | 引数ありで値を返す     |
| `AsyncQuery<TResult>`       | `SendQueryAsync<TQuery, TResult>()`          | `OnExecuteAsync(CancellationToken ct)`           | 非同期で値を返す      |
| `AsyncQuery<TArg, TResult>` | `SendQueryAsync<TQuery, TArg, TResult>(arg)` | `OnExecuteAsync(TArg arg, CancellationToken ct)` | 引数ありで非同期に値を返す |

## Event の書き方

Eventは、下位層からPresenterへ通知するときに使います。

Eventは命令ではなく、起きたことの通知です。

```csharp
using MyArchitecture.Core;

public readonly struct PlayerHpChangedEvent : IEvent
{
    public PlayerHpChangedEvent(int hp)
    {
        Hp = hp;
    }

    public int Hp { get; }
}
```

GameServiceまたはCommandから発行します。

```csharp
this.PublishEvent(new PlayerHpChangedEvent(nextHp));
```

Presenterで購読します。

```csharp
this.SubscribeEvent<PlayerHpChangedEvent>(
    eventData => _view.SetHp(eventData.Hp));
```

非同期処理を含む場合は、CancellationTokenを受け取るoverloadを使います。

```csharp
this.SubscribeEvent<PlayerHpChangedEvent>(
    async (eventData, ct) =>
    {
        await _view.PlayHpChangedAsync(eventData.Hp, ct);
    });
```

## ViewSignal と Observable の購読

ViewからPresenterへ入力を渡すときは `ViewSignal` を使います。

```csharp
this.SubscribeTo(
    _view.DamageClicked,
    () => this.SendCommand<DamagePlayerCommand, int>(10));
```

値付きのViewSignalは `ViewSignal<T>` を使います。

```csharp
this.SubscribeTo(
    _view.ItemSelected,
    itemId => this.SendCommand<SelectItemCommand, int>(itemId));
```

R3のObservableも `SubscribeTo` で購読できます。

```csharp
this.SubscribeTo(
    _model.Level,
    level => _view.SetLevel(level));
```

非同期処理を行う場合は `SubscribeToAsync` を使います。

```csharp
this.SubscribeToAsync(
    _model.Level,
    async (level, ct) =>
    {
        await _view.PlayLevelUpAsync(level, ct);
    });
```

連続入力を抑えたい場合は `SubscribeToThrottled` を使います。

```csharp
this.SubscribeToThrottled(
    _view.DamageClicked,
    () => this.SendCommand<DamagePlayerCommand, int>(10),
    TimeSpan.FromMilliseconds(300));
```

## 実装の基本パターン

1. View interfaceに入力用の `ViewSignal` と表示更新用メソッドを定義する
2. `View` を継承したView実装でUnityの入力、表示、演出を扱う
3. Modelを `partial class` で作り、状態を持たせる
4. Modelの公開getterやpublic readonly fieldからReadOnlyModelを自動生成する
5. GameServiceでModelを変更し、必要ならEventを発行する
6. CommandからGameServiceを呼ぶ
7. Queryで読み取り処理をまとめる
8. PresenterでViewSignal、ReadOnlyModel、Eventを購読する
9. LifetimeScopeで登録するか、自動登録に任せる

## よくある注意点

| 内容                                                 | 対応                                                       |
| -------------------------------------------------- | -------------------------------------------------------- |
| View実装で `IView` だけを実装している                          | View層の実装クラスは `View` を継承する                                |
| PresenterにModel本体をInjectしている                       | `IReadOnlyPlayerModel` のようなReadOnlyModelをInjectする        |
| ModelのReadOnly interfaceが生成されない                    | Modelに `partial` が付いているか確認する                             |
| Eventを命令として使っている                                   | Eventは起きたことの通知にする                                        |
| `Configure` を上書きしている                               | `ArchitectureLifetimeScope` では `RegisterModels` などを上書きする |
| Eventを登録していない                                      | `RegisterEvent<TEvent>()` するか、自動登録の対象になっているか確認する         |
| CommandやQueryが見つからない                               | `RegisterCommand<T>()` / `RegisterQuery<T>()` されているか確認する |
| CommandやQueryで `CommandBase` / `QueryBase` を継承している | 実装者は `Command` 系 / `Query` 系を継承する                        |
| GameServiceを独自interfaceでInjectしたい                  | 自動登録では独自interfaceに登録されないため、手動登録する                        |
| 同じView interfaceを複数のScene Viewが実装している              | 複数生成されるものはEntityViewとして扱うか、View interfaceを分ける            |
