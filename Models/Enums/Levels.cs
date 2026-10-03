namespace EduTechSpace.Models.Enums
{
	[Flags]
	public enum Levels
	{
		First =0,
		Second= 1, 
		Third = 2 ,
		Forth = 4,
        Teacher  = First | Second| Third| Forth // For teachers
	}
}
