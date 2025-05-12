namespace UniverseRift.GameModels.Heroes
{
	public class Characteristics
	{
		public int limitLevel;
		public float Damage;
		public float HP;
		public float Initiative;
		public float ProbabilityCriticalAttack;
		public float DamageCriticalAttack;
		public float Accuracy;
		public float CleanDamage;
		public float Dodge;
		public int CountTargetForSimpleAttack = 1;
		public int CountTargetForSpell = 1;
		public BaseCharacteristicModel Main = new();
	}
}
