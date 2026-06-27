using System.Linq;
using Content.Shared.Barks;
using Content.Shared.Examine;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.IdentityManagement;
using Content.Shared.Preferences;
using Robust.Shared.GameObjects.Components.Localization;
using Robust.Shared.Prototypes;

namespace Content.Shared.Humanoid;

public sealed partial class HumanoidProfileSystem : EntitySystem
{
    [Dependency] private IPrototypeManager _prototype = default!;
    [Dependency] private GrammarSystem _grammar = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HumanoidProfileComponent, ExaminedEvent>(OnExamined);
    }

    public void ApplyProfileTo(Entity<HumanoidProfileComponent?> ent, HumanoidCharacterProfile profile)
    {
        if (!Resolve(ent, ref ent.Comp))
            return;

        ent.Comp.Gender = profile.Gender;
        ent.Comp.Age = profile.Age;
        ent.Comp.Species = profile.Species;
        ent.Comp.Sex = profile.Sex;
        Dirty(ent);

        SetBarkVoice(ent, profile.BarkVoice);

        var sexChanged = new SexChangedEvent(ent.Comp.Sex, profile.Sex);
        RaiseLocalEvent(ent, ref sexChanged);

        if (TryComp<GrammarComponent>(ent, out var grammar))
        {
            _grammar.SetGender((ent, grammar), profile.Gender);
        }
    }

    /// <summary>
    /// Ensures the entity has a <see cref="SpeechSynthesisComponent"/> set to the given bark voice,
    /// falling back to a species-appropriate default if the requested voice isn't valid. Barks
    /// </summary>
    public void SetBarkVoice(Entity<HumanoidProfileComponent?> ent, ProtoId<BarkPrototype>? barkVoiceId)
    {
        if (!Resolve(ent, ref ent.Comp))
            return;

        var species = ent.Comp.Species;
        ProtoId<BarkPrototype> voice = HumanoidCharacterProfile.DefaultBarkVoice;

        if (barkVoiceId is { } id
            && _prototype.TryIndex<BarkPrototype>(id, out var bark)
            && (bark.SpeciesWhitelist == null || bark.SpeciesWhitelist.Contains(species)))
        {
            voice = id;
        }
        else
        {
            // Pick the first round-start voice valid for this species.
            var fallback = _prototype.EnumeratePrototypes<BarkPrototype>()
                .FirstOrDefault(o => o.RoundStart && (o.SpeciesWhitelist == null || o.SpeciesWhitelist.Contains(species)));

            if (fallback != null)
                voice = fallback.ID;
        }

        var comp = EnsureComp<SpeechSynthesisComponent>(ent);
        comp.VoicePrototypeId = voice;
        Dirty(ent, comp);
    }

    private void OnExamined(Entity<HumanoidProfileComponent> ent, ref ExaminedEvent args)
    {
        var identity = Identity.Entity(ent, EntityManager);
        var species = GetSpeciesRepresentation(ent.Comp.Species).ToLower();
        var age = GetAgeRepresentation(ent.Comp.Species, ent.Comp.Age);

        args.PushText(Loc.GetString("humanoid-appearance-component-examine", ("user", identity), ("age", age), ("species", species)));
    }

    /// <summary>
    /// Takes ID of the species prototype, returns UI-friendly name of the species.
    /// </summary>
    public string GetSpeciesRepresentation(ProtoId<SpeciesPrototype> species)
    {
        if (_prototype.TryIndex(species, out var speciesPrototype))
            return Loc.GetString(speciesPrototype.Name);

        Log.Error("Tried to get representation of unknown species: {speciesId}");
        return Loc.GetString("humanoid-appearance-component-unknown-species");
    }

    /// <summary>
    /// Takes ID of the species prototype and an age, returns an approximate description
    /// </summary>
    public string GetAgeRepresentation(ProtoId<SpeciesPrototype> species, int age)
    {
        if (!_prototype.TryIndex(species, out var speciesPrototype))
        {
            Log.Error("Tried to get age representation of species that couldn't be indexed: " + species);
            return Loc.GetString("identity-age-young");
        }

        if (age < speciesPrototype.YoungAge)
        {
            return Loc.GetString("identity-age-young");
        }

        if (age < speciesPrototype.OldAge)
        {
            return Loc.GetString("identity-age-middle-aged");
        }

        return Loc.GetString("identity-age-old");
    }
}
