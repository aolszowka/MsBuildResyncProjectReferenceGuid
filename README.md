# MsBuildResyncProjectReferenceGuid
Resyncs the Project Reference Guid to what is in the referenced project

## Background
In a large source tree it is very possible to get into a situation where the Guid of the Referenced Project has changed.

Consider the following:

1. Developer duplicates ProjectGuid in another Project
2. Developer tries to load both of these Projects into the same Visual Studio Solution
3. Visual Studio choses one of the projects as the "loser" and regenerates its Guid.
4. The "loser" project was referenced (via ProjectReferences) in projects other than those loaded into the solution when Visual Studio "fixed" the duplication.

These additional reference projects now have the wrong Guid in them. However if the relative path is correct Visual Studio and MSBuild will still successfully "build". HOWEVER all of the configuration information stored within any other Solution Files is now invalid. This is because the Guid is used in the Solution File Format as a Key when setting configurations. 

## When To Use This Tool
This tool scans the given directory and resyncs the Guid within the ProjectReference to what is actually in the project file.

As a side effect of this process if the Project that is referenced in ProjectReference no longer exists this utility throws an `InvalidOperationException`. If you know that the project exists (but perhaps has just been relocated to a new location) you can use the sister utility https://github.com/aolszowka/MsBuildProjectReferenceFixer

If the project was completely deleted you should remove the reference all together to avoid MsBuild becoming confused and attempting to build something that does not exist.

## Usage
```text
Scans given directory for MsBuild Projects; Resycing the ProjectReference Project Guid to the referenced Project.
Invalid Command/Arguments. Valid commands are:

[directory]                   - [MODIFIES] Spins through the specified directory
                                and all subdirectories for Project files resetting
                                the ProjectReference Project tag. Prints modified
                                paths. ALWAYS Returns 0.
validatedirectory [directory] - [READS] Spins through the specified directory
                                and all subdirectories for Project files prints
                                all projects whose ProjectReference Project tags
                                should be updated. Returns the number of invalid projects.
```

## Hacking
The most likely change you will want to make is changing the supported project files. In theory this tool should support any MSBuild Project Format that utilizes a ProjectGuid.

See ResyncProjectReferenceGuid.GetProjectsInDirectory(string) for the place to modify this.

The tool should also support those projects that utilize the same ProjectReference format as CSPROJ formats.

## Contributing
Pull requests and bug reports are welcomed so long as they are MIT Licensed.

## License
This tool is MIT Licensed.