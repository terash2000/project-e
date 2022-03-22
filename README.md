# Coding Standard

## The reason that we must have it with our project
Coding standardization can be seen everywhere in the software industry, hence, it's best that we must practice in pursuing to be a professional software developer (even in such a small project like this). There're several markdown benefits that it'll provide us with. Here's the list.

 - Consistent code secures the scalability and the maintainability of our project.
 - Providing readability, therefore, reduce the time that will be wasted trying to understand parts of the project.
 - Intuitive code will let us see the relevant section of the code, what is linking together or referring to each other.
 - Overall, making our developing environment less stressful and has fewer arguments over coding styles.

## General rules of thumb

 1. There's no solid rule on when to use `MonoBehavior` or not but it's generally good to avoid using it in every class. Use it only when you need that class to interact with GameObject.
2. Magic number should be avoided. Declare it as a `const` variable and assign a meaningful name to it (even if it's long).
```cs
// Wrong
if (_boss._health < 40.0f) // 40.0f is considered a magic number.
{
	_boss.ChangeStage();
}

// Right
private const float BossStageOneHealthThreshold = 40.0f; // Now we know what 40.0f means.
if (_boss._health < BossStageOneHealthThreshold)
{
	_boss.ChangeStage();
}
```
 3. A class should be doing its purposes and its fields and methods should be related to each other. (High Cohesion)
 4. A class should not be trying to call another class's methods if it's unnecessary. (Low Coupling)
(ps. flashback to our software engineer class)
## Naming Convention
### Cases
 - All `public` and `protected` fields must be in **PascalCase**.
 - All `class`, `records`, `struct` and class methods must be in **PascalCase**.
 - All `interface` must start with **I** and the rest of the name must be in **PascalCase**.
 - All `private` and `internal` fields must be in **camelCase**.
 - All `private` must start with **_**
 - All `private static` must start with **s_** and thread static with **t_** following with **camelCase**.
### Naming
 - Abbreviation will cause confusion and should be avoided e.g. no `rh` for RaycastHit, no `mc` for MeshCollider.
 - Float variable should be in decimal and followed by **f** e.g. `40.0f`.
 - Boolean variable should start with an associated verb e.g. `_isPoisoned` `_hasMoved` `_canCastSpell`
 - Event-related method should always start with **On** e.g. `OnDead`

## Layout Convention
All of the guideline below is taken from [**Microsoft Coding Convention**](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)

 - Always use default IDE formatter settings (it usually format with Microsoft standard of C#). This will help you with proper whitespace between brackets, parentheses, mathematic operation, and boolean expression.
 - One statement per line. One declaration per line.
 - Continuation line should be indented and one indentation (one tab) should be 4 characters long.
 - Use parentheses to make clauses in the boolean expression more apparent.
 - Curly bracket should start on a new line.
 - After the end of the curly bracket, leave one new line after it (except for `if/else` and `try/catch`)
```cs
// Example
if (condition)
{
	...
}
else
{
	...
}

foreach(element e in array1)
{
	...
}

foreach(element e in array2)
{
	...
}
```
## Extensive Elements Organization
Since Microsoft doesn't provide coding convention with elements, every classes elements should be written in the following order according to [**C# Google Style Guide**](https://google.github.io/styleguide/csharp-style.html) with some **modifications**. Some elements are Unity-specific elements so I'll add them where it's the most appropriate.
Note that some elements from this Styleguide are grouped together, hence I'll ungroup it and put it in the order that should make sense the most
### Within classes, structs and interfaces
 - Enums
 - Delegates
 - Events (not to be confused with Methods that is "event-related" mentioned above)
 - Constant Fields
 - Static Fields
 - Readonly Fields
 - Serialize Fields (Unity-specific)
 - Fields
 - Properties (Such as shorthanded getter and setter)
 - Indexers (Similar to properties but for arrays, read more at Microsoft pages)
 - Constructors
 - Finalizers (Destructors)
 - Methods
 - Interfaces
 - Structs
 - Classes
### Fields and method, ordered by accessibility
 - Public
 - Internal
 - Protected internal
 - Protected
 - Private
### Elements Layout
 - Each and every group, either by element or accessibility should be followed with **one** blank line.
 - You should always give `private` accessibility to every fields and methods as much as possible, even if you want another class to access the field. For fields, create getter and setter. For methods, just change it to `public`. This also apply to inherited method `Start()` and `Update()` from `MonoBehavior`.
 - Short getter and setter properties can use the same line curly bracket for both opening and closing
 - [SerializeField] should be put in separate line from field declaration.
```cs
// All examples of cases. Imagining you're developing Hades
public class PlayerManager : MonoBehavior
{
	public static int EscapeAttemps;
	public static int TotalDarkness;

	private static int s_nectar;
	private static int s_diamond;
	private static int s_ambrosia;
	private static int s_ruby;

	private Player _player;
	...
	public void OnDead
	{
		if (_player.DeathDefiance > 0)
		{
			_player.DeathDefiance--;
			Revive();
		}
	}
	...
}

public class Player : Entity, IControllable
{
	[SerializeField]
	private GameObject _zagreusPrefabs;
	
	private int _health;
	private sbyte _deathDefiance;
	private sbyte _soulCast;
	private bool _isPoisoned;

	public Health
	{
		get { return _health; }
		set { _health = value; }
	}
	
	public DeathDefiance
	{
		get { return _deathDefiance; }
		set { _deathDefiance= value; }
	}
	
	void Start()
	{
	...
	}
	...
}
```
## Commenting Convention

 - Comments should always be in the separate line.
 - Always begin comment text with one whitespace and an uppercase letter.
 - End comment with a proper fullstop.
```
// This is a proper comment. If the comment is too long for one line you can
// do something like this.
```
## Singleton Usage with Unity
Singleton is pretty straightforward by just declaring some static fields and setting it to `this`  inside the Awake() method.
```cs
public class Singleton : MonoBehavior
{
	public static Singleton Instance;
	
	private void Awake()
	{
		Instance = this;
	}
}
```
Using simpleton with Unity is not that simple and it's come with the following problems.
 - Can lead to multiple instances of the same singletons.
 - Usually, we just put singleton class and attach to a GameObject that's created inside one scene, may be a few other scenes too but this will not persist whenever we change scenes.
 - The order that each `Awake()` will be called is unknown. It's not recommended to call `Singleton.Instance` in any `Awake()` which can read to Null Reference Exception.
```cs
public class Singleton : MonoBehavior
{
	private static Singleton _instance;
	
	public static Singleton Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<Singleton>();
				if (_instance == null)
				{
					GameObject newSingletonObject = new GameObject();
					newSingletonObject.name = "Singleton";
					_instance = newSingletonObject.AddComponent<Singleton>();
					DontDestroyOnLoad(newSingletonObject);
				}
				return _instance;
			}
		}
	}
	
	private void Awake()
	{
		if (_instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else if (_instance != this)
		{
			Destroy(gameObject);
		}
	}
}
```
 - To tackle all problems above, we must write a somewhat complicated singleton class and therefore we must copy and paste all of this code to another singleton class
```cs
public class GenericSingleton<T> : MonoBehavior where T : Component
{
	private static T _instance;
	
	public static T Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<T>();
				if (_instance == null)
				{
					GameObject newSingletonObject = new GameObject();
					newSingletonObject.name = typeof(T).name
					_instance = newSingletonObject.AddComponent<T>();
					DontDestroyOnLoad(newSingletonObject);
				}
				return _instance;
			}
		}
	}
	
	private void Awake()
	{
		if (_instance == null)
		{
			instance = this as T;
			DontDestroyOnLoad(gameObject);
		}
		else if (_instance != this as T)
		{
			Destroy(gameObject);
		}
	}
}
```

## Credits and Read More
 - Our senior's Github, [**pjumruspun**](https://github.com/pjumruspun)
 - [**Microsoft Coding Convention**](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
 - [**C# Google Style Guide**](https://google.github.io/styleguide/csharp-style.html)
 - [**StyleCop Rules Documentation**](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/tree/master/documentation)
 - [**UnityGeeks, Singleton Unity 3D C#**](http://www.unitygeek.com/unity_c_singleton/)

