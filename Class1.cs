using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    public enum player_t {black, white, neither};

    public virtual class SpotBase {
        public virtual SpotBase();
        public virtual SpotBase(int x, int y);
        public virtual player_t getValue();
        public virtual void setValue(player_t player);
    }

    public class BoardBase
    {
        public virtual BoardBase();
        public virtual BoardBase(BoardBase board);

        public virtual void move(SpotBase spot);
        public virtual SpotBase getSpot(int x, int y);
        public virtual int getCaptures(player_t player);
        public virtual int getMoveNumber();
        public virtual player_t getCurrentPlayer();
        public virtual bool isGameOver();
        public virtual player_t getWinner();
    }

    public class AI
    {
    }
}
