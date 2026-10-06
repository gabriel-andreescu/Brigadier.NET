# Changelog

The format is based on [Keep a Changelog](https://keepachangelog.com/en/2.0.0/).

## [Unreleased]

### Changed

- `CommandSyntaxException.BuiltInExceptions` can be replaced, as in Mojang's Brigadier, so
  an application can word the built-in parse errors itself.

### Fixed

- When an argument type fails with an exception other than `CommandSyntaxException`, the
  parse error it becomes keeps that exception as its `InnerException`, so an application
  can tell a broken argument type from bad input and log its stack.

Earlier releases are documented in the
[upstream release history](https://github.com/AtomicBlom/Brigadier.NET/releases).
