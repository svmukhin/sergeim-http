# Changelog

## [0.6.0](https://github.com/svmukhin/sergeim-http/compare/v0.5.0...v0.6.0) (2026-07-21)


### Features

* add ManagedHttpWire with per-request HttpClient creation ([17f21c4](https://github.com/svmukhin/sergeim-http/commit/17f21c4cb6e412a1961b5a15edb679251fe0f11c))


### Bug Fixes

* add ConfigureAwait(false) to all async calls ([297f153](https://github.com/svmukhin/sergeim-http/commit/297f15316355f32559a694bbfdf4188f252dce7d))


### Documentation

* add HttpClient Lifecycle section and update wire examples ([45c5557](https://github.com/svmukhin/sergeim-http/commit/45c55578d854a5d9148e2f43dd1abeebbfd71d86))
* convert samples to async, add sync methods warning ([6b0a337](https://github.com/svmukhin/sergeim-http/commit/6b0a3376499b80565093eff64392481d77d5674f))


### Code Refactoring

* configure editorconfig, autoformat warnings ([69c958a](https://github.com/svmukhin/sergeim-http/commit/69c958aa8658f526845027d92e2ff151b088bafa))
* extract HTTP request building into WireHelper ([99ed52d](https://github.com/svmukhin/sergeim-http/commit/99ed52d73cfae146a5c876f4c1a475c32d3b914f))


### Tests

* add ManagedHttpWire decorator chain compatibility test ([8ebcf7f](https://github.com/svmukhin/sergeim-http/commit/8ebcf7ff5704409a31230f5c9a8b7a3863d346bb))
* add ManagedHttpWire tests with per-request client verification ([02ee569](https://github.com/svmukhin/sergeim-http/commit/02ee569674be9ff551bd9beecf1c9e52c48fe81a))


### Miscellaneous

* **#14:** fix version prefix in ReleasePackageNotes link ([a5a6dd9](https://github.com/svmukhin/sergeim-http/commit/a5a6dd96d30a0143decb77cfdacee2d979a8dbb0))
* add lock files, reorder CI steps, disable debug symbols in Release ([730f1e8](https://github.com/svmukhin/sergeim-http/commit/730f1e8682103363ea0e82d0b94eeee2e14fc446))
* added build tag tot Miscellaneous section ([7b3cc79](https://github.com/svmukhin/sergeim-http/commit/7b3cc7984448a7cac8f0b85ed04bbe6338259e00))
* fix code formatting ([3538018](https://github.com/svmukhin/sergeim-http/commit/35380184a204a7b2ea4a518d9b49896c67d553a0))
* multi-target net8.0 and net10.0, use --locked-mode restore ([000fb3d](https://github.com/svmukhin/sergeim-http/commit/000fb3dedc6137ba514728a9f5a7453aa8e5a654))

## [0.5.0](https://github.com/svmukhin/sergeim-http/compare/0.4.1...v0.5.0) (2026-06-24)


### Features

* **#10:** switch to NuGet trusted publishing (OIDC, keyless) ([aba9a86](https://github.com/svmukhin/sergeim-http/commit/aba9a86be62b7fdebc64f13623abc0d7026c8cf5))


### Miscellaneous

* **#9:** add release-please workflow, REUSE compliance, and markdownlint config ([9eef95b](https://github.com/svmukhin/sergeim-http/commit/9eef95ba58e08607548576c412c81f9214db0b58))
