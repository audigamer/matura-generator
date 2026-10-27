# MaturaGenerator

MaturaGenerator is a tool that helps students prepare for bulgarian state exams (Maturas) by procedurally generating unlimited variations of exam problems based on templates and instantly checking their answers.

This is work in progress and serves as my course project for the **ASP.NET Fundamentals** course at SoftUni.


## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/)
- Microsoft SQL Server / LocalDB / SQLEXPRESS
    
### Setup

1. Clone the repository.
2. Check `appsettings.json` and adjust the connection string to your local SQL Server if necessary.
3. Apply database migrations:

   `dotnet ef database update --project MaturaGenerator`
4. Run the project:

   `dotnet run --project MaturaGenerator`



## Key Features

### Problem Templates and Generation

Problem generation is done through Problem Templates and their structure & constraints.

- **Statements** define the problem text and math structures, e.g. `The solution of the equation [@Ax + @B = @C] is:`. The math structures are inside square brackets `[]`.
- **Constants** are the placeholders within the Structure for randomly chosen numbers, e.g. `@A`, `@X`.
- **Domains** control the range and numeric type of generated constants.
- **Invariants** define the requirements of the generated problem, such as the constants being nonzero or the answer - an integer.
- **Answer Rules** let the problem creator specify the formula for the answer and input type (slider or multiple choice)

Problem creators can make Problem Templates from the **Create** section from the website.

### Practicing

Every Problem Template can be practiced by the user. This generates a problem from the template and its properties, neatly visualizes it by converting the math structures to LaTeX syntax, and allows the user to submit and check a solution within a given time limit.

## Tech Stack

- **Target Framework:** .NET 10 (ASP.NET Core MVC)
- **Database & ORM:** Microsoft SQL Server, Entity Framework Core 10 (Code-First Migrations)
- **Mathematical Evaluation:** [NCalc](https://github.com/ncalc/ncalc)
- **Frontend / Rendering:** Razor Views, Bootstrap 5, [KaTeX](https://katex.org/) (LaTeX math typesetting), jQuery
  Validation
- **Architecture:** Multi-project solution:
    - `MaturaGenerator`: ASP.NET Core MVC web application, Controllers, Views, EF Core context, and Services.
    - `MaturaGenerator.MathEngine`: Domain-specific AST, parser, transforms, and LaTeX rendering pipeline.

## Software Design & Architecture


### 1. Algebraic Data Types (ADTs)

Instead of having many nullable or potentially conflicting property states, I chose to make the code unable to encode such impossible states by using ADTs and pattern matching in a way similar to Rust programming language's idioms.

States are modeled as small class hierarchies using C# records, with private constructors to close the hierarchy. This means pattern matching with switch statements is trivial (the compiler even warns if not all types are accounted for when using this C# idiom), and invalid states literally cannot exist:

- **`PracticeSessionViewModel`**: Either `Active` (with session ID and problem) or `Error` (with
template and error description).
- **`AnswerCheckResult`**: Closed hierarchy for `Correct`, `Incorrect`, `TimeExpired`, and`SessionNotFound`.
- **`GeneratedAnswerInput`**: Separate type for how the answer is picked: either `Slider(min, max)` or`MultipleChoice(choices)`.


### 2. Math Engine & Syntax Tree Pipeline (`MaturaGenerator.MathEngine`)
I chose to parse formulas and syntax like square brackets `[...]` with an abstract syntax tree (AST) rather than using regex string replacement,
because it very quickly becomes a headache to deal with, especially with things like simplifying math and turning it into LaTeX.

The MathEngine project uses the following parsing pipeline:
1. **`MathLexer` & `MathParser`**: Turn expressions into an Abstract Syntax Tree (AST).
2. **`ConstantSubstitutor`**: Walks the AST and swaps placeholders like `@A` with the sampled numbers.
3. **`MathSimplifier`**: Cleans up weird math artifacts (turns things like `1x` into `x`, `+ -3` into `-3`, and removes `0x`).
4. **`LatexRenderer`**: Walks the final tree and converts it into LaTeX so KaTeX can display it nicely in the browser.


### 3. Clean OOP Architecture
Aside from following the SOLID principles, I also used these specific design ideas in my project:

- **Composition over Inheritance**: Instead of letting classes grow way too big with an obscene amount of properties, I
chose to compose classes inside properties for DTOs and ViewModels, and EF collections for Entities.
- **Table-Per-Hierarchy (TPH) Inheritance**: EF Core stores polymorphic entities in single tables with discriminator columns:
- `DomainConstraint` -> `IntDomain` and `FractionDomain`
- `AnswerRule` -> `CustomAnswerRule` (slider) and `MultiChoiceAnswerRule` (multiple choice)
- **Dependency Injection & Decoupling**: Controllers never talk directly to EF Core `MaturaDbContext`. All database operations go through `IProblemTemplateService`, and math generation is handled by `IProblemGeneratorService` and `IPracticeService`.