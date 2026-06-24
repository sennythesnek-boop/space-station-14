// SPDX-FileCopyrightText: 2025 Aviu00 <93730715+Aviu00@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aviu00 <aviu00@protonmail.com>
// SPDX-FileCopyrightText: 2025 GoobBot <uristmchands@proton.me>
// SPDX-FileCopyrightText: 2025 gluesniffler <linebarrelerenthusiast@gmail.com>
// SPDX-FileCopyrightText: 2026 issyman182 <issyman182@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// Ported into the iss14 fork from Goobstation (Content.Goobstation.Common.Movement.MoverControllerEvents).

namespace Content.Shared.Movement.Events;

/// <summary>
/// Ported from Goobstation: raised on an entity when it toggles walk/run mode.
/// Used by the sprint feature to cancel sprinting when the player toggles walk.
/// </summary>
public readonly record struct ToggleWalkEvent(bool Walking);
