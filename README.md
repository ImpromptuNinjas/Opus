# ImpromptuNinjas.Opus

This is a C# project combined with a static build of the libopus native library
that provides an interface to the Ogg Opus audio codec.

There are minor customizations to the build of the native Opus library to
better facilitate the C# bindings.

## Features

- Provides a wrapper for the Opus library functions.
- Handles Opus error codes and converts them into human-readable strings.
- Provides a method to get the version string of the Opus library.

## Usage

To use this library, you need to create an instance of the `OpusDecoder` class and call its methods.

## Requirements

- .NET Standard 1.1 or greater
- Windows, Linux (glibc or musl) _OSX temporarily not supported_
- AMD64 or ARM64 architecture

## Building

To build the C# project, you need to have .NET SDK installed on your machine. After that, you can use the `dotnet build` command to build the project.

To build the native runtime components you need Docker or Podman with docker alias support.

## Contributing

Contributions are welcome. Please open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License.
