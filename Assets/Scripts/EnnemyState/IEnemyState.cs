namespace EnemyState
{
    public interface IEnemyState
    {
        void Enter(EnemyScript enemy);   // Appel� � l'entr�e dans l'�tat
        void Update(EnemyScript enemy);  // Appel� � chaque frame
        void Exit(EnemyScript enemy);    // Appel� � la sortie de l'�tat
        void OnHit(EnemyScript enemy, float damage); // Pour r�agir � un coup re�u
    }
}