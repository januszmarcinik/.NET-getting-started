using System;
using System.ComponentModel;

namespace NET.GettingStarted.Entities
{
    public class TeamMember
    {
        private string _name;
        private Role _role;
        private int _grade;

        public TeamMember(Guid id, string name, Role role, int grade)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            Name = name;
            Role = role;
            Grade = grade;
        }

        public Guid Id { get; }

        public string Name
        {
            get => _name;
            set => _name = value ?? throw new ArgumentNullException(nameof(Name));
        }

        public Role Role
        {
            get => _role;
            set
            {
                if (!Enum.IsDefined(typeof(Role), value))
                {
                    throw new InvalidEnumArgumentException(nameof(Role), (int)value, typeof(Role));
                }
                _role = value;
            }
        }

        public int Grade
        {
            get => _grade;
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(Grade));
                _grade = value;
            }
        }

        private bool Equals(TeamMember other) => 
            Id == other.Id;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TeamMember) obj);
        }

        public override int GetHashCode() => 
            Id.GetHashCode();
    }
}
