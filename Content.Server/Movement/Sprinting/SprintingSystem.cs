// SPDX-FileCopyrightText: 2025 August Eymann <august.eymann@gmail.com>
// SPDX-FileCopyrightText: 2025 GoobBot <uristmchands@proton.me>
// SPDX-FileCopyrightText: 2025 gluesniffler <linebarrelerenthusiast@gmail.com>
// SPDX-FileCopyrightText: 2026 issyman182 <issyman182@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later
//
// Ported into the iss14 fork from Goobstation (Content.Goobstation.Server.Sprinting).
//
// Goobstation's server subclass added a sprinter-vs-sprinter StartCollide knockdown that only
// triggered against entities with ActiveSandevistanUserComponent. Sandevistan is not ported to this fork,
// so that handler is intentionally omitted and this is a plain concrete instantiation of the shared system.

using Content.Shared.Movement.Sprinting;

namespace Content.Server.Movement.Sprinting;

public sealed class SprintingSystem : SharedSprintingSystem
{
}
