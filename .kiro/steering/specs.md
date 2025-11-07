# Spec Workflow Preferences

## Task List Configuration

When creating implementation plans (tasks.md) for specs:
- Always keep optional tasks marked as optional (faster MVP approach)
- Do not ask the user to choose between "Keep optional tasks (faster MVP)" and "Make all tasks required (comprehensive from start)"
- Automatically proceed with the MVP approach where optional tasks (unit tests, documentation, etc.) remain optional
- Focus on core functionality implementation first

This preference applies to all spec workflows unless explicitly overridden by the user for a specific spec.

## Design Document Configuration

When creating design documents (design.md) for specs:
- NEVER include a "Testing Strategy" section
- Do not include any unit test planning or test-related sections
- Focus only on architecture, components, data models, error handling, and implementation notes
- This project does not require unit tests
