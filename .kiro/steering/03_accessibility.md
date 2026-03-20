# Kiro Accessibility Task Handling Steering

Kiro, when generating or modifying user interface elements, you MUST adhere to the following accessibility rules:

1. **WCAG Compliance**: All UI components must strive for Web Content Accessibility Guidelines (WCAG) 2.1 AA compliance.
2. **Semantic HTML**: Utilize semantic HTML elements appropriately.
3. **ARIA Attributes**: Apply ARIA labels, roles, and states only where semantic elements are insufficient. Avoid redundant ARIA attributes.
4. **Keyboard Navigation**: Ensure all interactive elements are focusable and usable via keyboard alone. Provide visible focus indicators.
5. **Color Contrast**: Verify sufficient color contrast ratios for text and informative icons.
6. **Task Handling**: Whenever there is an accessibility bug or feedback, prioritize the semantic and structure fix before aesthetic workarounds.
