# Bannerlord Expanded Template

**Bannerlord Expanded Template** is an utility mod for *Mount & Blade II: Bannerlord*. This framework enhances the
functionality of existing XML templates in the game, providing modular C# components and an integrated logging system
for easier debugging and development.

## Features

- **Expanded Equipment API**:
    - Supports siege equipment templates.
    - Allows multiple equipment pools.

For more details, check out the [Expanded Equipment API Documentation](docs/expanded-equipment-api.md).

## Usage

Unlike traditional utility mods, *Bannerlord Expanded Template* isn't distributed as a standalone Bannerlord module. The rationale is to avoid the complexity and errors associated with requiring users to download multiple mods. Instead, it's recommended to distribute this mod's binaries directly within your own mod's binary folder.

This method leverages Bannerlord's ability to load multiple `SubModules` from a single mod, improving reliability and reducing hassle.

### Integration Methods

#### Via XML

1. In your `SubModule.xml` file, add the following node under the `SubModules` section:

   ```xml
   <SubModule>
        <Name value="Bannerlord.ExpandedTemplate" />
        <DLLName value="Bannerlord.ExpandedTemplate.Integration.dll" />
        <SubModuleClassType value="Bannerlord.ExpandedTemplate.Integration.ExpandedTemplateSubModule" />
        <Tags/>
   </SubModule>
   ```

2. Extract the mod's binaries into your `bin\Win64_Shipping_Client` folder (and `Gaming.Desktop.x64_Shipping_Client` if you support GamePass).

#### Via C#

1. **Add the mod as a dependency**: Reference the NuGet library in your C# project by adding this to your `.csproj` file:

   ```xml
   <PackageReference Include="Bannerlord.ExpandedTemplate.API" Version="1.0.0" />
   ```

2. **Mod Initialization**: In your C# `SubModule` constructor, initialize the expanded template:

   ```cs
   public class MySubModule : MBSubModuleBase
   {
       public MySubModule()
       {
           new BannerlordExpandedTemplateApi().Bind();
       }
   }
   ```

For more details about integrating logging and using the C# API, check out the [C# API Documentation](docs/csharp-api.md).

## Contributing

Contributions are welcome! If you find any bugs or have ideas for improvement, feel free to submit an issue or create a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.
