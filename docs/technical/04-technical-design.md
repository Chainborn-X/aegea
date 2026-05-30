# Technical Design

## Purpose

This document will capture technical decisions as the project moves from vision and prototypes toward production.

Early technical work should protect the creative goals:

- Fast iteration on top-down environments.
- Support for painterly 2D art direction.
- Strong tooling for maps, tiles, props, encounters, dialogue, and asset validation.
- Source-controlled project structure.
- Clear separation between experiments and production code.

## Initial Technical Priorities

- Choose an engine after prototyping movement, camera, combat, map streaming, and content workflow.
- Establish asset naming and import conventions before large asset production.
- Keep prototype code disposable unless explicitly promoted.
- Make tools serve designers and artists, not only programmers.
- Track decisions in `docs/technical`.

## Future Decisions

- Engine choice.
- Rendering approach.
- Tilemap versus hand-authored scene composition.
- Asset pipeline.
- Dialogue and localization system.
- Save system.
- Build and release process.
- Automated validation for content.
