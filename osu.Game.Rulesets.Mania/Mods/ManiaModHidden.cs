﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Localisation;
using osu.Game.Rulesets.Mania.UI;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Mania.Mods
{
    public partial class ManiaModHidden : ManiaModWithPlayfieldCover
    {
        /// <summary>
        /// osu!stable is referenced to 768px.
        /// </summary>
        private const float reference_playfield_height = 768;

        private const float min_coverage = 160 / reference_playfield_height;
        private const float max_coverage = 400f / reference_playfield_height;
        private const float coverage_increase_per_combo = 0.5f / reference_playfield_height;

        public override LocalisableString Description => @"Keys fade out before you hit them!";
        public override double ScoreMultiplier => 1;

        public override Type[] IncompatibleMods => base.IncompatibleMods.Concat(new[]
        {
            typeof(ManiaModFadeIn),
            typeof(ManiaModCover)
        }).ToArray();

        public override BindableNumber<float> Coverage { get; } = new BindableFloat(min_coverage);
        protected override CoverExpandDirection ExpandDirection => CoverExpandDirection.AgainstScroll;

        private readonly BindableInt combo = new BindableInt();

        public override void ApplyToScoreProcessor(ScoreProcessor scoreProcessor)
        {
            base.ApplyToScoreProcessor(scoreProcessor);

            combo.UnbindAll();
            combo.BindTo(scoreProcessor.Combo);
            combo.BindValueChanged(c => Coverage.Value = Math.Min(max_coverage, min_coverage + c.NewValue * coverage_increase_per_combo), true);
        }

        protected override PlayfieldCoveringWrapper CreateCover(Drawable content) => new LegacyPlayfieldCover(content);

        private partial class LegacyPlayfieldCover : PlayfieldCoveringWrapper
        {
            public LegacyPlayfieldCover(Drawable content)
                : base(content)
            {
            }

            protected override float GetHeight(float coverage)
            {
                if (DrawHeight == 0)
                    return base.GetHeight(coverage);

                return base.GetHeight(coverage) * reference_playfield_height / DrawHeight;
            }
        }
    }
}
