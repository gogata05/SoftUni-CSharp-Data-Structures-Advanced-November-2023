using System;
using System.Collections.Generic;
using System.Linq;

namespace _02.FitGym
{
    public class FitGym : IGym
    {
        private readonly Dictionary<int, Member> members;
        private readonly Dictionary<int, Trainer> trainers;
        private readonly Dictionary<int, int> memberToTrainer;

        public FitGym()
        {
            this.members = new Dictionary<int, Member>();
            this.trainers = new Dictionary<int, Trainer>();
            this.memberToTrainer = new Dictionary<int, int>();
        }

        public void AddMember(Member member)
        {
            if (this.members.ContainsKey(member.Id))
            {
                throw new ArgumentException();
            }

            this.members.Add(member.Id, member);
        }

        public void HireTrainer(Trainer trainer)
        {
            if (this.trainers.ContainsKey(trainer.Id))
            {
                throw new ArgumentException();
            }

            this.trainers.Add(trainer.Id, trainer);
        }

        public void Add(Trainer trainer, Member member)
        {
            if (!this.trainers.ContainsKey(trainer.Id) || this.memberToTrainer.ContainsKey(member.Id))
            {
                throw new ArgumentException();
            }

            if (!this.members.ContainsKey(member.Id))
            {
                this.members.Add(member.Id, member);
            }

            this.memberToTrainer.Add(member.Id, trainer.Id);
        }

        public bool Contains(Member member)
        {
            return this.members.ContainsKey(member.Id);
        }

        public bool Contains(Trainer trainer)
        {
            return this.trainers.ContainsKey(trainer.Id);
        }

        public Trainer FireTrainer(int id)
        {
            if (!this.trainers.TryGetValue(id, out var trainer))
            {
                throw new ArgumentException();
            }

            this.trainers.Remove(id);

            var membersToRemove = this.memberToTrainer.Where(kvp => kvp.Value == id).Select(kvp => kvp.Key).ToList();
            foreach (var memberId in membersToRemove)
            {
                this.memberToTrainer.Remove(memberId);
            }

            return trainer;
        }

        public Member RemoveMember(int id)
        {
            if (!this.members.TryGetValue(id, out var member))
            {
                throw new ArgumentException();
            }

            this.members.Remove(id);
            this.memberToTrainer.Remove(id);

            return member;
        }

        public int MemberCount => this.members.Count;

        public int TrainerCount => this.trainers.Count;

        public IEnumerable<Member> GetMembersInOrderOfRegistrationAscendingThenByNamesDescending()
        {
            return this.members.Values
                .OrderBy(m => m.RegistrationDate)
                .ThenByDescending(m => m.Name);
        }

        public IEnumerable<Trainer> GetTrainersInOrdersOfPopularity()
        {
            return this.trainers.Values.OrderBy(t => t.Popularity);
        }

        public IEnumerable<Member> GetTrainerMembersSortedByRegistrationDateThenByNames(Trainer trainer)
        {
            return this.memberToTrainer
                .Where(kvp => kvp.Value == trainer.Id)
                .Select(kvp => this.members[kvp.Key])
                .OrderBy(m => m.RegistrationDate)
                .ThenBy(m => m.Name);
        }

        public IEnumerable<Member> GetMembersByTrainerPopularityInRangeSortedByVisitsThenByNames(int lo, int hi)
        {
            return this.memberToTrainer
                .Where(kvp => this.trainers.ContainsKey(kvp.Value) && this.trainers[kvp.Value].Popularity >= lo && this.trainers[kvp.Value].Popularity <= hi)
                .Select(kvp => this.members[kvp.Key])
                .OrderBy(m => m.Visits)
                .ThenBy(m => m.Name);
        }

        public Dictionary<Trainer, HashSet<Member>> GetTrainersAndMemberOrderedByMembersCountThenByPopularity()
        {
            return this.trainers.Values
                .OrderBy(t => this.memberToTrainer.Count(kvp => kvp.Value == t.Id))
                .ThenBy(t => t.Popularity)
                .ToDictionary(
                    t => t,
                    t => new HashSet<Member>(this.memberToTrainer.Where(kvp => kvp.Value == t.Id).Select(kvp => this.members[kvp.Key]))
                );
        }
    }
}
