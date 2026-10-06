# Changelog

The format is based on [Keep a Changelog](https://keepachangelog.com/en/2.0.0/).

## [Unreleased]

### Changed

- `CommandSyntaxException.BuiltInExceptions` can be replaced, as in Mojang's Brigadier, so
  an application can word the built-in parse errors itself.
- **Breaking:** `IBuiltInExceptionProvider` has a new `EnumInvalid` member, which
  `EnumArgumentType` raises with the text it read and the enum's names. Implement it in
  your own provider.

### Fixed

- An invalid `EnumArgumentType` value reported the text it read as the literal it expected,
  and gave no position in the input.

Earlier releases are documented in the
[upstream release history](https://github.com/AtomicBlom/Brigadier.NET/releases).
