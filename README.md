# Crossguard ⚔️🛡️

> A physics- and skill-driven melee duel prototype exploring true swing-plane weapon control and graduated shield defense. Built with Unity 6 (URP).

[![Engine](https://img.shields.io/badge/Unity-6%20(URP)-blue.svg)](#)
[![Status](https://img.shields.io/badge/Status-14--Day%20Feasibility%20Sprint-orange.svg)](#)
[![Control](https://img.shields.io/badge/Input-Gamepad%20First-green.svg)](#)

---

## 🎯 The Core Hook
- **True Swing-Plane Sword Tracking:** Dual-stick / mouse vector maps directly to your blade's 2D cutting angle. No animation-locked swings or binary 3-lane limits.
- **Graduated Shield Defense:** Instant spatial guard snapping via bumpers and triggers (`LB`, `RB`, `LT`, `RT`). Defense is non-binary: the tighter your shield angle matches the incoming cut vector, the less durability you lose.
- **Toy Appeal First:** Built from the ground up to feel kinetic and responsive in a greybox room before adding final art.

---

## 🎮 Prototype Controls (Gamepad First)

```
        [ LB: High Left ]          [ RB: High Right ]
        [ LT: Low Left  ]          [ RT: Low Right  ]
```

- **Left Stick:** Duelist Footwork / Movement
- **Right Stick:** 2D Sword Swing-Plane Vector (`X, Y` clock face)
- **LB / RB / LT / RT:** Directional Shield Stance Snapping (spatial quadrant layout)
- *(Optional Chords: LB + RB = High Center / Overhead, LT + RT = Low Center)*

---

## 📐 Combat Mathematics

- **Strike Vector:** $\hat{V}_{\text{strike}}$ (normal to blade axis in swing direction)
- **Shield Vector:** $\hat{V}_{\text{shield}}$ (braced normal of shield orientation)
- **Angle Delta:** $\theta = \arccos(|\hat{V}_{\text{strike}} \cdot \hat{V}_{\text{shield}}|)$
- **Match Ratio:** $M = \text{Clamp01}(1 - \frac{\theta}{\theta_{\max}})$

| Feedback Parameter | Clean Hit ($M = 0$) | Weak Block ($M \approx 0.5$) | Perfect Block ($M \approx 1.0$) |
| :--- | :--- | :--- | :--- |
| **Audio** | Dull flesh/leather impact | Low, grinding crunch / heavy scrape | Razor-sharp bell-ring / resonant *CLANG* |
| **Audio Pitch** | $1.0\times$ (Fixed) | $0.85\times$ (Heavy, dragged down) | $1.25\times$ - $1.4\times$ (High-frequency ping) |
| **Hitstop** | 1–2 frames (fluid cut) | 4–5 frames (heavy friction) | 6–8 frames (dramatic kinetic dead-stop) |
| **Shield Durability** | $0\%$ lost (Player takes HP damage) | $25–40\%$ shield gauge loss | $< 5\%$ shield gauge loss |
| **Recoil Impulse** | Minor follow-through | Moderate mutual stagger | Heavy sword deflection recoil |

---

## 🗓️ 14-Day Feasibility Sprint

- **Days 1–3:** Core verb (sword swing-plane tracking) + hook (angle-matched shield defense) functional and juiced in greybox.
- **Days 4–7:** AI opponent using symmetric stance/swing logic.
- **Days 8–10:** Movement integration and dodge/step.
- **Days 11–13:** Blind playtesting (2–3 people, 3 minutes, zero instructions).
- **Day 14:** Hard kill / proceed decision.
