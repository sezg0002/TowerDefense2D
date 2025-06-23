namespace EnemyState
{
    public interface IEnemyState
    {
        void Enter(EnemyScript enemy);   // Appelé à l'entrée dans l'état
        void Update(EnemyScript enemy);  // Appelé à chaque frame
        void Exit(EnemyScript enemy);    // Appelé à la sortie de l'état
        void OnHit(EnemyScript enemy, float damage); // Pour réagir à un coup reçu
    }
}