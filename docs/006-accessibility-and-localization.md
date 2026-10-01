# Accessibility and localization

## Accessibility

Accessibility is part of feature completion for the Web/PWA experience. UI changes should:

- Work with keyboard-only navigation and provide a visible, logical focus order.
- Use semantic HTML and accessible names for controls and links.
- Preserve sufficient text and control contrast and avoid color as the sole carrier of meaning.
- Provide meaningful headings, landmarks, labels, and error messages.
- Respect zoom/reflow and reduced-motion preferences.
- Be checked with automated tooling and manual keyboard/screen-reader review appropriate to the change.

Automated checks are useful but do not establish accessibility conformance by themselves. Record known limitations in the PR.

## Localization

- Keep user-facing text out of domain and application logic.
- Use the localization project and established resource patterns rather than hard-coded UI strings.
- Format dates, times, numbers, and units according to the active locale.
- Do not concatenate translated fragments to construct sentences.
- Add or update tests/resources when user-visible copy or locale behavior changes.

The initial supported locales and translation workflow must be confirmed in a product issue; do not imply broader language support than has been implemented.
