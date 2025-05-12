namespace UniverseRift.GameModels.Heroes
{
	public class BaseCharacteristicModel
	{
		public int Attack;
		public int Defense;
		public int Speed;
		public TypeMovement MovementType;
		public bool Mellee;
		public TypeStrike AttackType;
		public bool CanRetaliation = true;
		public int CountCouterAttack = 1;

	}
}
