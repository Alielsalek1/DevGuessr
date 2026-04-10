# Design System Specification: High-Fidelity Cyber-Zen
 
## 1. Overview & Creative North Star: "The Precision Glitch"
This design system is a study in high-contrast digital minimalism. Our Creative North Star is **The Precision Glitch**: the intentional marriage of hyper-clean, matte surfaces with volatile, high-energy digital artifacts. 
 
While the aesthetic draws from "glitch" culture, the execution must remain sophisticated and "editorial." We break the standard grid-based "SaaS template" look through **intentional asymmetry**—offsetting labels, using aggressive typography scales, and employing "pixel leaks" (1px lines that extend beyond container boundaries) to create a sense of controlled chaos. This is not a messy system; it is a surgical one.
 
---
 
## 2. Colors & Surface Architecture
The palette is rooted in a deep obsidian foundation, allowing our neon accents to serve as functional light sources rather than mere decoration.
 
### The Palette
- **Background/Surface**: `#0E0E0E` (The Matte Void)
- **Primary (Magenta)**: `#FF7CF5` — Used for critical actions and brand expression.
- **Secondary (Electric Violet)**: `#D078FF` — Used for interactive states and secondary flows.
- **Tertiary (Neon Cyan)**: `#00FFFF` — Used exclusively for data highlights and "system ready" indicators.
 
### The "No-Line" Rule & Surface Nesting
Standard UI uses borders to separate content; we use **Tonal Depth**.
*   **Prohibition**: Never use solid 1px borders to define major sections. 
*   **The Layering Principle**: Use the `surface-container` hierarchy to create depth. A `surface-container-low` (#131313) card should sit on the `surface` (#0E0E0E) background. For inner content, use `surface-container-high` (#201F1F). 
*   **The Glass Rule**: Floating modals or navigation rails must use `surface-container-lowest` with a 20% opacity and a `24px` backdrop-blur. This "frosted obsidian" effect ensures the UI feels like a physical layer of light-emitting glass.
 
---
 
## 3. Typography: Technical Brutalism
Our typography pairs the expressive, geometric personality of **Space Grotesk** with the utilitarian precision of **JetBrains Mono**.
 
*   **Display & Headlines (Space Grotesk)**: These are the "voice" of the interface. Use `display-lg` (3.5rem) with tight letter-spacing (-0.02em) to create an authoritative, editorial feel.
*   **Technical Data & Labels (JetBrains Mono)**: All functional data, micro-copy, and status indicators must use JetBrains Mono. This reinforces the "system-level" aesthetic, making the user feel like they are interacting with a high-end terminal.
*   **Hierarchy**: Use extreme contrast. A `headline-lg` should often be paired with a `label-sm` technical readout to create a "spec-sheet" aesthetic.
 
---
 
## 4. Elevation & Digital Texture
In this system, "Elevation" is not achieved through traditional shadows, but through **Light Pollution** and **Chromatic Aberration**.
 
*   **Tonal Layering**: Instead of `box-shadow`, use background shifts. To lift a component, move from `surface-dim` to `surface-bright`.
*   **The Ghost Border**: If a container requires a boundary (e.g., an input field), use the `outline-variant` token at 15% opacity. It should feel like a faint laser line, not a stroke.
*   **Signature Interaction (Chromatic Aberration)**: On hover states for primary buttons or cards, apply a 1px offset shadow: Magenta (`#FF00FF`) at -1px and Cyan (`#00FFFF`) at 1px. This creates a "shimmer" effect that suggests a digital glitch without compromising legibility.
*   **Pixel Leaks**: Occasionally extend a 1px vertical or horizontal line from the corner of a container exactly 16px into the gutter. This mimics a "rendering error" and adds to the high-fidelity aesthetic.
 
---
 
## 5. Components
 
### Buttons: High-Energy Triggers
*   **Primary**: Solid `primary` (#FF7CF5) with `on-primary` (#580058) text. 0px corner radius. On hover, the background shifts to `primary-dim` with a 2px "pixel leak" on the bottom-right corner.
*   **Secondary/Ghost**: 1px `outline-variant` (at 20% opacity). Text in Space Grotesk. Hover triggers the Chromatic Aberration shadow effect.
*   **Tertiary (Technical)**: JetBrains Mono text with a `tertiary` (#00FFFF) underline that is only 2px tall.
 
### Cards & Containers
*   **Style**: No borders. Use `surface-container-low`.
*   **Structure**: Forbid the use of horizontal dividers. Separate content using `32px` or `48px` of vertical whitespace.
*   **Header**: Use a `label-sm` in JetBrains Mono at the top-left of every card as a "System ID" or category tag.
 
### Input Fields
*   **Base**: `surface-container-highest` background. Bottom-border only (1px, `outline-variant`).
*   **Focus**: Bottom-border transitions to `primary` (#FF7CF5). A small "pixel leak" line appears at the edge of the input.
 
### Tooltips & Overlays
*   **Execution**: Use "Frosted Obsidian" (Low opacity + Backdrop blur).
*   **Typography**: All tooltip content must be JetBrains Mono, `label-md`.
 
---
 
## 6. Do’s and Don'ts
 
### Do:
*   **Embrace Asymmetry**: Align text to the left but place technical metadata in the far top-right corner.
*   **Use Mono for Numbers**: Any numerical data must be JetBrains Mono to ensure tabular alignment and a "data-heavy" look.
*   **Sharp Edges**: Every corner must be `0px`. Rounding is strictly prohibited; it breaks the "Zen/Glitch" tension.
 
### Don’t:
*   **No Blue Backgrounds**: The only cyan/blue allowed is the sharp `#00FFFF` accent. Never use blue for surfaces or large gradients.
*   **No Standard Shadows**: Avoid the "soft grey shadow" look of traditional Material Design. Use background color shifts or high-saturation neon "glows" (8% opacity) instead.
*   **No Clutter**: The "Glitch" is a seasoning, not the main course. If a screen feels "busy," remove the glitch lines and focus on the Space Grotesk typography.
 
---
 
## 7. Implementation Tokens
| Role | Token Value | Usage |
| :--- | :--- | :--- |
| **Foundation** | `#0E0E0E` | Main application background |
| **Active Highlight** | `#FF7CF5` | Primary actions, focal points |
| **System Info** | `#00FFFF` | Success states, data labels, technical highlights |
| **Muted Text** | `#ADAAAA` | De-emphasized labels, JetBrains Mono metadata |
| **Layer 1** | `#131313` | Cards, secondary containers |
| **Layer 2** | `#262626` | Hover states, inner nested containers |