using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShaceShip
{
    struct Position
    {
        public int col;
        public int rol;
        public Position(int Col, int Rol)
        {
            this.col = Col;
            this.rol = Rol;
        }
    }


    class Program
    {
        static Position ship = new Position(15, 39);
        static List<Position> bullets = new List<Position>();
        static List<Position> enemies = new List<Position>();
        static Random enemy = new Random();


        static void Main(string[] args)
        {
            Console.SetWindowSize(30, 40);
            Console.SetBufferSize(30, 40);
            int counter = 0;

            while (true)
            {
                UpdateField();
                if (counter == 20)
                {
                    GenerateEnemies();
                    counter = 0;
                }

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo userKey = Console.ReadKey();

                    if ((userKey.Key == ConsoleKey.LeftArrow) && ship.col > 0)
                    {
                        ship = new Position(ship.col - 1, ship.rol);
                    }
                    if ((userKey.Key == ConsoleKey.RightArrow) && ship.col < Console.WindowWidth - 3)
                    {
                        ship = new Position(ship.col + 1, ship.rol);
                    }

                    if (userKey.Key == ConsoleKey.Spacebar)
                    {
                        bullets.Add(new Position(ship.col, ship.rol - 1));
                    }
                }

                Draw();
                Thread.Sleep(100);
                Console.Clear();
                counter++;
            }
        }

        private static void GenerateEnemies()
        {
            //enemies.Add(new Position(15, 0));
            //enemies.Add(new Position(20, 0));
            enemies.Add(new Position(enemy.Next(30), 0));
        }

        private static void UpdateField()
        {
            UpdateShots();
            UpdateEnemies();
            RemoveDestroyedEnmy();
        }

        private static void RemoveDestroyedEnmy()
        {
            if (enemies.Count >= 1)
            {
                for (int i = 0; i < bullets.Count; i++)
                {
                    for (int j = 0; j < enemies.Count; j++)
                    {
                        if ((bullets[i].col == enemies[j].col) && (enemies[j].rol > bullets[i].rol))
                        {
                            enemies.Remove(enemies[j]);
                            if (enemies.Count == 0)
                            {
                                break;
                            }
                        }
                    }
                }
            }

        }

        private static void UpdateEnemies()
        {
            if (enemies.Count != 0)
            {
                for (int i = 0; i <= (enemies.Count - 1); i++)
                {
                    enemies[i] = new Position(enemies[i].col, enemies[i].rol + 1);
                    if (enemies[i].rol > Console.WindowHeight - 1)
                    {
                        enemies.Remove(enemies[i]);
                    }
                }
            }
        }

        private static void UpdateShots()
        {
            if (bullets.Count != 0)
            {
                for (int i = (bullets.Count - 1); i >= 0; i--)
                {
                    bullets[i] = new Position(bullets[i].col, bullets[i].rol - 1);
                    if (bullets[i].rol < 0)
                    {
                        bullets.Remove(bullets[i]);
                    }
                }
            }

        }

        static void Draw()
        {
            DrawShot();
            DrawShip();
            DrawEnemy();
        }

        private static void DrawEnemy()
        {
            enemies.ForEach(x => DrawSymbolAtPosition(x.col, x.rol, '@', ConsoleColor.Magenta));
        }

        private static void DrawShot()
        {
            bullets.ForEach(x => DrawSymbolAtPosition(x.col, x.rol, '|', ConsoleColor.Yellow));
        }

        private static void DrawShip()
        {
            if (ship.col > 0)
            {
                DrawSymbolAtPosition(ship.col - 1, ship.rol, '#', ConsoleColor.Black);
            }
            if (ship.col < Console.WindowWidth)
            {
                DrawSymbolAtPosition(ship.col + 1, ship.rol, '#', ConsoleColor.Black);
            }
            DrawSymbolAtPosition(ship.col, ship.rol, '#', ConsoleColor.White);
        }

        static void DrawSymbolAtPosition(int col, int rol, char symbol, ConsoleColor color)
        {
            Console.SetCursorPosition(col, rol);
            Console.ForegroundColor = color;
            Console.Write(symbol);
        }
    }
}
