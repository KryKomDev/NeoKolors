// NeoKolors
// Copyright (c) krystof 2026

using System.Diagnostics;

namespace NeoKolors.Tui.Core;

public static class Profiler {

    public static class Global {

        public static long Screen_Total_Count {
            get {
                SumScreens();

                return field;
            }
            private set;
        }

        public static TimeSpan Screen_Total {
            get {
                SumScreens();

                return field;
            }
            private set;
        }

        public static TimeSpan Render_Image {
            get {
                SumScreens();

                return field;
            }
            private set;
        }

        public static TimeSpan Render_Chars {
            get {
                SumScreens();

                return field;
            }
            private set;
        }

        internal static void SumScreens() {
            var count = 0L;
            var total = TimeSpan.Zero;
            var render_image = TimeSpan.Zero;
            var render_chars = TimeSpan.Zero;

            for (int i = 0; i < SCREEN_PROFILERS.Count; i++) {
                var p = SCREEN_PROFILERS[i];
                count += p.Count;
                total += p.Total;
                render_image += p.RenderImage;
                render_chars += p.RenderChars;
            }

            Screen_Total_Count = count;
            Screen_Total       = total;
            Render_Image       = render_image;
            Render_Chars       = render_chars;
        }

        private static readonly List<ScreenProfiler> SCREEN_PROFILERS = [];
            
        internal static void Register(ScreenProfiler screenProfilerProfiler) {
            SCREEN_PROFILERS.Add(screenProfilerProfiler);
        }
    }

    internal class ScreenProfiler {
        
        private          long      _count;
        private readonly Stopwatch _total        = new();
        private readonly Stopwatch _render_image = new();
        private readonly Stopwatch _render_chars = new();

        public long     Count => _count;
        public TimeSpan Total       => _total.Elapsed;
        public TimeSpan RenderImage => _render_image.Elapsed;
        public TimeSpan RenderChars => _render_chars.Elapsed;

        [Conditional("NK_RENDERING_PROFILING")]
        internal void Start_Screen_Total() {
            _count++;
            _total.Start();
        }

        [Conditional("NK_RENDERING_PROFILING")]
        internal void Stop_Screen_Total() {
            _total.Stop();
        }

        [Conditional("NK_RENDERING_PROFILING")]
        internal void Start_Screen_RenderImage() {
            _render_image.Start();
        }

        [Conditional("NK_RENDERING_PROFILING")]
        internal void Stop_Screen_RenderImage() {
            _render_image.Stop();
        }

        [Conditional("NK_RENDERING_PROFILING")]
        internal void Start_Screen_RenderChars() {
            _render_chars.Start();
        }

        [Conditional("NK_RENDERING_PROFILING")]
        internal void Stop_Screen_RenderChars() {
            _render_chars.Stop();
        }
    }
}