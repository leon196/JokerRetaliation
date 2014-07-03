using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class IA : MonoBehaviour {

    public struct Node
    {
        public GameObject pos;
        public GameObject parent;
        public float cost;
    }

    Vector3 _pos;
    List<GameObject> path = new List<GameObject>();
    int _costHorizon = 10;
    int _costDiag = 15;
    float _widthBloc;
    float _offsetY = 0.3f;
    string str = string.Empty;
    Dictionary<string, List<GameObject>> _paths = new Dictionary<string, List<GameObject>>();

	// Use this for initialization
	void Start () {
        Manager.Instance.Scroll.PrepareIA();
        str = "Level_0";

        StartCoroutine("Movement");
	}
	
	// Update is called once per frame
	void Update () {

        if (this.transform.parent == null ||this.transform.parent.parent == null ||
            this.transform.parent.parent.name.Length < 5 || Manager.Instance.IsGameOver)
            return;

        string t = this.transform.parent.parent.name.Substring(0, 5);
        if (t.Equals("Level"))
        {
            if (!this.transform.parent.parent.name.Equals(str))
            {
                str = this.transform.parent.parent.name;
                print(str);
            }
        }
	}

    IEnumerator Movement()
    {

        foreach (var n in _paths)
        {
            print(n.Key);
        }

        this.transform.parent = _paths[str][0].transform;
        this.transform.localPosition = new Vector3(0.0f, this.collider.bounds.size.y - _offsetY, 0.0f);
        
        while (!Manager.Instance.IsGameOver)
        {
            path = new List<GameObject>(_paths[str]);
            // PathFind();
            while (path.Count > 0)
            {
                this.transform.parent = path[0].transform;
                var from = this.transform.localPosition;
                var to = new Vector3(0.0f, this.collider.bounds.size.y - _offsetY, 0.0f);

                float step = 0.0f;
                float rate = 11f;
                float smoothStep = 0.0f;
                float lastStep = 0.0f;
                while (step < 1.0)
                {
                    step += Time.deltaTime * rate;
                    smoothStep = Mathf.SmoothStep(0.0f, 1.0f, step);
                    this.transform.localPosition = Vector3.Slerp(from, to, smoothStep);
                    lastStep = smoothStep;
                    yield return new WaitForSeconds(0.05f);
                }

                if (step > 1.0)
                {
                    this.transform.localPosition = to;
                }

                path.RemoveAt(0);
            }
            str = Manager.Instance.Scroll.GetNextLv().name;
        }
    }

    public void PathFind(List<GameObject> obstacles, string nameLv)
    {
        _widthBloc = obstacles.First().collider.bounds.size.x;
        float zBloc = obstacles.First().collider.bounds.size.z;

        List<Vector3> allNodes = new List<Vector3>();

        // Init all
        float semiBlocs = _widthBloc / 2;
        var walkableBlocs = obstacles.Where(o => !obstacles.Where(o2 =>
            o2.transform.position != o.transform.position).Any(o2 =>
                (o2.transform.position.x >= o.transform.position.x - semiBlocs && o2.transform.position.x <= o.transform.position.x + semiBlocs) &&
                o2.transform.position.y >= o.transform.position.y + semiBlocs && o2.transform.position.y <= o.transform.position.y + _widthBloc + semiBlocs)).ToList();
        var obstacleBlocs = obstacles.Except(walkableBlocs).ToList();
        var firstBloc = walkableBlocs.Where(o => o.transform.tag.Equals("StartBloc")).First();
        var lastBloc = walkableBlocs.Where(o => o.transform.tag.Equals("EndBloc")).First();
        
        // startingNode = new Node() { pos = firstBloc, cost = 0 };
        Node startingNode = new Node() { cost = 0, parent = null, pos = firstBloc };
        Dictionary<Node, float> openList = new Dictionary<Node, float>();
        openList.Add(startingNode, 0);
        List<Node> closedList = new List<Node>();
        bool foundTheEnd = false;

        while (!foundTheEnd && openList.Count > 0)
        {
            // On se place la où le coût est le plus faible
            var currentNode = openList.Aggregate((n1, n2) =>
                n1.Value < n2.Value ? n1 : n2);

            openList.Remove(currentNode.Key);
            closedList.Add(currentNode.Key);

            var t = obstacles.Where(o => o.transform.position != currentNode.Key.pos.transform.position).ToList();
            /*foreach (var o in t)
                print(o.transform.position);*/
            List<Node> neighbors = FindPossibleNodes3(currentNode.Key, t, walkableBlocs, closedList);

            foreach (var n in neighbors)
            {
                if (!openList.Any(nv => nv.Key.pos == n.pos))
                {
                    float val = n.cost + calculEstimateMovement(n.pos.transform.position, lastBloc.transform.position);
                    openList.Add(n, val);
                }
                else
                {
                    var nv = openList.Where(p => p.Key.pos == n.pos).First();
                    if (n.cost > nv.Key.cost)
                    {
                        openList.Remove(nv.Key);
                        float val = n.cost + calculEstimateMovement(n.pos.transform.position, lastBloc.transform.position);
                        openList.Add(n, val);
                    }
                }
            }

            foundTheEnd = currentNode.Key.pos == lastBloc ? true : false;
        }

        // print("Closed " + closedList.Count);
        Node node = closedList.Last();
        List<GameObject> pos = new List<GameObject>();
        pos.Add(node.pos);

        while (node.cost != 0)
        {
            pos.Add(node.parent);
            node = closedList.Where(n => n.pos == node.parent).First();
        }
        pos.Reverse();
        path = pos;
        _paths.Add(nameLv, pos);
    }

    List<Node> FindPossibleNodes3(Node current, List<GameObject> obstacles, List<GameObject> walkableArea, List<Node> closed)
    {
        List<Node> returnList = new List<Node>();
        Node addNode;

        // Check le premier à droite
        float semiBloc = _widthBloc / 2;
        var walkableRight = walkableArea.Where(o =>
            o.transform.position.x >= current.pos.transform.position.x + semiBloc &&
            o.transform.position.x <= current.pos.transform.position.x + _widthBloc + semiBloc).ToList();
        var obstacleRight = obstacles.Where(o =>
            o.transform.position.x >= current.pos.transform.position.x + semiBloc &&
            o.transform.position.x <= current.pos.transform.position.x + _widthBloc + semiBloc).ToList();

        if (walkableRight.Count > 0)
        {
            foreach (var b in walkableRight)
            {
                // Si plus de deux blocs au dessus
                if (b.transform.position.y >= current.pos.transform.position.y + (_widthBloc * 2) + semiBloc)
                    continue;

                bool haveFound = false;

                // Si le bloc est en bas, on regarde si on peut sauter sur un autre bloc
                if (b.transform.position.y <= current.pos.transform.position.y - semiBloc)
                {
                    var furtherRight = walkableArea.Where(o =>
                        o.transform.position.x >= b.transform.position.x + semiBloc &&
                        o.transform.position.x <= b.transform.position.x + _widthBloc + semiBloc).ToList();

                    bool haveObstacleB = obstacles.Any(o =>
                                (o.transform.position.x >= (current.pos.transform.position.x - semiBloc) &&
                                o.transform.position.x <= (current.pos.transform.position.x + semiBloc))
                                &&
                                (o.transform.position.y >= (current.pos.transform.position.y + semiBloc) &&
                                o.transform.position.y <= (current.pos.transform.position.y + _widthBloc*3 + semiBloc)));
                    foreach (var br in furtherRight)
                    {
                        var furtherRightRight = walkableArea.Where(o =>
                        o.transform.position.x >= br.transform.position.x + semiBloc &&
                        o.transform.position.x <= br.transform.position.x + _widthBloc + semiBloc).ToList();
                        
                        bool haveObstacleBR = obstacles.Any(o =>
                            (o.transform.position.x >= (b.transform.position.x - semiBloc) &&
                            o.transform.position.x <= (b.transform.position.x + semiBloc))
                            &&
                            (o.transform.position.y >= (current.pos.transform.position.y + semiBloc) &&
                            o.transform.position.y <= (current.pos.transform.position.y + _widthBloc * 3 + semiBloc)));

                        foreach (var brr in furtherRightRight)
                        {
                            // Si au dessus
                            if (brr.transform.position.y >= current.pos.transform.position.y + semiBloc)
                                continue;

                            bool haveObstacleBRR = obstacles.Any(o =>
                                (o.transform.position.x >= br.transform.position.x - semiBloc &&
                                o.transform.position.x <= br.transform.position.x + semiBloc)
                                &&
                                (o.transform.position.y >= current.pos.transform.position.y + semiBloc &&
                                o.transform.position.y <= current.pos.transform.position.y + _widthBloc * 3 + semiBloc));

                            if (haveObstacleB || haveObstacleBR || haveObstacleBRR)
                            {
                                print("OBSTACLE");
                                continue;
                            }

                            addNode = new Node();
                            addNode.cost = calculEstimateMovement(current.pos.transform.position, brr.transform.position);
                            addNode.parent = current.pos;
                            addNode.pos = brr;
                            returnList.Add(addNode);
                            haveFound = true;
                        }

                        if(!haveFound)
                        {
                            // Si plus d'un bloc au dessus
                            if (br.transform.position.y >= current.pos.transform.position.y + _widthBloc + semiBloc)
                                continue;

                            if(haveObstacleB || haveObstacleBR)
                                continue;
                        
                            addNode = new Node();
                            addNode.cost = calculEstimateMovement(current.pos.transform.position, br.transform.position);
                            addNode.parent = current.pos;
                            addNode.pos = br;
                            returnList.Add(addNode);
                        }
                        
                    }
                }
                
                if(!haveFound)
                {

                    addNode = new Node();
                    addNode.cost = calculEstimateMovement(current.pos.transform.position, b.transform.position);
                    addNode.parent = current.pos;
                    addNode.pos = b;
                    returnList.Add(addNode);
                }
            }
        }

        return returnList;
    }
    /*
    List<Node> FindPossibleNodes(Node current, List<Vector3> obstacles, List<Vector3> allNodes, List<Node> closed)
    {
        bool obstacleRight = obstacles.Any(o =>
            (o.x >= current.pos.x + _offset && o.x <= current.pos.x + _offset * 2)
            &&
            (o.y >= current.pos.y && o.y <= current.pos.y + _offset)
            );
        bool obstacleLeft = obstacles.Any(o =>
            (o.x <= current.pos.x - _offset && o.x <= current.pos.x - _offset * 2)
            &&
            (o.y >= current.pos.y && o.y <= current.pos.y + _offset)
            );
        bool obstacleTop = obstacles.Any(o =>
            (o.x >= current.pos.x && o.x <= current.pos.x + _offset)
            &&
            (o.y >= current.pos.y + _offset && o.y <= current.pos.y + _offset * 2)
            );
        bool obstacleBottom = obstacles.Any(o =>
            (o.x >= current.pos.x && o.x <= current.pos.x + _offset)
            &&
            (o.y >= current.pos.y - _offset && o.y <= current.pos.y - _offset * 2)
            );

        List<Node> returnList = new List<Node>();
        
        Node addNode = new Node();

        if (!obstacleRight)
        {
            addNode.pos = new Vector3(current.pos.x + _offset, current.pos.y, current.pos.z);
            addNode.parent = current.pos;
            addNode.cost = _costHorizon;
            returnList.Add(addNode);
        }

        if (!obstacleLeft)
        {
            addNode.pos = new Vector3(current.pos.x - _offset, current.pos.y, current.pos.z);
            addNode.parent = current.pos;
            addNode.cost = _costHorizon;
            returnList.Add(addNode);
        }

        if (!obstacleTop)
        {
            addNode.pos = new Vector3(current.pos.x, current.pos.y + _offset, current.pos.z);
            addNode.parent = current.pos;
            addNode.cost = _costHorizon;
            returnList.Add(addNode);

            if (!obstacleRight)
            {
                addNode.pos = new Vector3(current.pos.x + _offset, current.pos.y + _offset, current.pos.z);
                addNode.parent = current.pos;
                addNode.cost = _costDiag;
                returnList.Add(addNode);
            }

            if (!obstacleLeft)
            {
                addNode.pos = new Vector3(current.pos.x - _offset, current.pos.y + _offset, current.pos.z);
                addNode.parent = current.pos;
                addNode.cost = _costDiag;
                returnList.Add(addNode);
            }
        }

        if (!obstacleBottom)
        {
            addNode.pos = new Vector3(current.pos.x, current.pos.y - _offset, current.pos.z);
            addNode.parent = current.pos;
            addNode.cost = _costHorizon;
            returnList.Add(addNode);

            if (!obstacleRight)
            {
                addNode.pos = new Vector3(current.pos.x + _offset, current.pos.y - _offset, current.pos.z);
                addNode.parent = current.pos;
                addNode.cost = _costDiag;
                returnList.Add(addNode);
            }

            if (!obstacleLeft)
            {
                addNode.pos = new Vector3(current.pos.x - _offset, current.pos.y - _offset, current.pos.z);
                addNode.parent = current.pos;
                addNode.cost = _costDiag;
                returnList.Add(addNode);
            }
        }

        return returnList.Where(n => !(closed.Any(n2 => n.pos == n2.pos))).ToList();
    }*/

    float calculEstimateMovement(Vector3 current, Vector3 target)
    {
        float xDistance = Mathf.Abs(current.x - target.x);
        float yDistance = Mathf.Abs(current.y - target.y);

        if (xDistance < yDistance)
        {
            return _costDiag * yDistance + _costHorizon * (xDistance - yDistance);
        }
        else
        {
            return _costDiag * xDistance + _costHorizon * (yDistance - xDistance);
        }
    }
}
