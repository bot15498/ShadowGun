using System;

namespace LibAudio {
    public readonly struct DB {
        public DB(float value) {
            Value = value;
            Percent = MathF.Sqrt(MathF.Pow(10f, Value / 10f));
        }

        public float Value {get;}
        public float Percent {get;}
    }

    public readonly struct Semitone {
        public Semitone(float value) {
            Value = value;
            Percent = MathF.Pow(2, Value / 12);
        }

        public float Value {get;}
        public float Percent {get;}
    }
}
