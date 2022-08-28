# Giinn

Neat console user input.


## Examples

### Enter input

Code:
```csharp
bool ValidateName(string? name) => name is "Bob";

var name = Input.Enter(
    label: "What is your name?",
    validator: ValidateName,
    retryMessage: "Your name must be Bob!",
    maximumRetries: 2
);
```
Output:
```
What is your name?
```
If you don't enter Bob: 
```
Your name must be Bob!
2 attmepts remaining.
What is your name? 
```


### Pick from a given set of options
Code: 
```csharp
var options = new [] { "Cat", "Dog", "Mouse", "Bunny", "Cow", "Rooster" };

var (index, pick) = Input.Pick(
    options: options,
    label: "Choose your favorite animal:",
    selector: " ->",
    limitRows: 4,
    startAtIndex: 1
);
```

Output: 
```
Choose your favorite animal:
    Cat
 -> Dog
    Mouse
    ...
```
Press down-arrow:
```
Choose your favorite animal:
    Cat
    Dog
 -> Mouse
    ...
```
Press down-arrow:
```
Choose your favorite animal:
    ...
    Mouse
 -> Bunny
    ...
```
Press enter:
```
Choose your favorite animal: Bunny
```

Pressing up from the top or down from the bottom wraps around to the other end:

```
Choose your favorite animal:
 -> Cat
    Dog
    Mouse
    ...
```

Press up:
```
Choose your favorite animal:
    ...
    Bunny
    Cow
 -> Rooster
```

### Confirm something
Code: 
```csharp
var willMarry = Input.Confirm(
    label: "Will you marry me?",
    @default: true
);
```

Output:
```
Will you marry me? (Y/n): 
```
Hitting enter without typing anything will result in whatever `@default` is. 

