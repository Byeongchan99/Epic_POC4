# Unity 씬 세팅 가이드

## 📋 수동 세팅 단계

### 1️⃣ GameManager 오브젝트 생성

1. **Hierarchy 우클릭** → `Create Empty`
2. 이름: `GameManager`
3. **Add Component** → `GameManager` 스크립트 추가

**Inspector 설정:**

#### References
- **Magic Circle Prefab**: `Assets/Prefabs/MagicCircle` (아래에서 생성)
- **Slash Detector**: SlashDetector 오브젝트 드래그 (아래에서 생성)
- **Status Text**: Canvas > StatusText 드래그
- **Timer Text**: Canvas > TimerText 드래그
- **Score Text**: Canvas > ScoreText 드래그

#### Game Settings
- **Cast Time**: `3` (마법 시전 시간)
- **Cuts Needed To Win**: `3` (성공에 필요한 자르기 횟수)
- **Spawn Position**: `(0, 0)` (마법진 생성 위치)

---

### 2️⃣ SlashDetector 오브젝트 생성

1. **Hierarchy 우클릭** → `Create Empty`
2. 이름: `SlashDetector`
3. **Add Component** → `SlashDetector` 스크립트 추가

**Inspector 설정:**
- **Slash Width**: `0.15` (자르는 선 두께)
- **Slash Color**: 빨간색 `(255, 76, 76, 255)`
- **Trail Duration**: `0.3` (잔상 지속 시간)
- **Cut Effect Prefab**: (선택사항, 비워둬도 됨)

---

### 3️⃣ MagicCircle Prefab 생성

1. **Hierarchy 우클릭** → `Create Empty`
2. 이름: `MagicCircle`
3. **Add Component** → `MagicCircle` 스크립트 추가

**Inspector 설정:**
- **Draw Duration**: `3` (그려지는 시간)
- **Circle Color**: 보라색 `(128, 51, 255, 255)`
- **Line Width**: `0.1` (선 두께)
- **Pattern**: `Circle` (기본값, 게임 시작 시 자동 변경됨)

4. **Hierarchy에서 MagicCircle을 Project > Assets/Prefabs 폴더로 드래그** (Prefab 생성)
5. **Hierarchy의 MagicCircle 삭제** (Prefab만 남김)

---

### 4️⃣ UI 생성

#### Canvas 생성
1. **Hierarchy 우클릭** → `UI > Canvas`

#### Status Text (상태 메시지)
1. **Canvas 우클릭** → `UI > Text`
2. 이름: `StatusText`
3. **Inspector 설정:**
   - Text: `Stage 1 - Cut 0/3 lines!`
   - Font Size: `32`
   - Alignment: 가운데 정렬
   - Color: 흰색
   - **Rect Transform:**
     - Anchor: Center
     - Pos X: `0`, Pos Y: `200`
     - Width: `600`, Height: `100`

#### Timer Text (타이머)
1. **Canvas 우클릭** → `UI > Text`
2. 이름: `TimerText`
3. **Inspector 설정:**
   - Text: `Time: 3.0s`
   - Font Size: `48`
   - Alignment: 가운데 정렬
   - Color: 빨간색 `(255, 128, 128, 255)`
   - **Rect Transform:**
     - Anchor: Top Center
     - Pos X: `0`, Pos Y: `-50`
     - Width: `300`, Height: `80`

#### Score Text (점수)
1. **Canvas 우클릭** → `UI > Text`
2. 이름: `ScoreText`
3. **Inspector 설정:**
   - Text: `Score: 0`
   - Font Size: `24`
   - Alignment: 왼쪽 위
   - Color: 흰색
   - **Rect Transform:**
     - Anchor: Top Left
     - Pos X: `150`, Pos Y: `-25`
     - Width: `300`, Height: `50`

#### Instruction Text (조작 안내)
1. **Canvas 우클릭** → `UI > Text`
2. 이름: `InstructionText`
3. **Inspector 설정:**
   - Text: `Click and drag to slash through magic circles!`
   - Font Size: `20`
   - Alignment: 가운데 아래
   - Color: 회색 `(179, 179, 179, 255)`
   - **Rect Transform:**
     - Anchor: Bottom Center
     - Pos X: `0`, Pos Y: `30`
     - Width: `600`, Height: `50`

---

### 5️⃣ Camera 설정

**Main Camera** 선택 후 Inspector:
- **Position**: `(0, 0, -10)`
- **Projection**: `Orthographic`
- **Size**: `5`
- **Background**: 어두운 파란색 `(13, 13, 26, 255)`

---

### 6️⃣ 최종 연결

1. **GameManager** Inspector로 돌아가기
2. **References 섹션 채우기:**
   - `Magic Circle Prefab`: Project에서 MagicCircle prefab 드래그
   - `Slash Detector`: Hierarchy의 SlashDetector 드래그
   - `Status Text`: Canvas > StatusText 드래그
   - `Timer Text`: Canvas > TimerText 드래그
   - `Score Text`: Canvas > ScoreText 드래그

---

## ✅ 완료 체크리스트

- [ ] Hierarchy에 GameManager 있음
- [ ] Hierarchy에 SlashDetector 있음
- [ ] Assets/Prefabs/MagicCircle.prefab 있음
- [ ] Canvas와 4개의 Text 있음
- [ ] GameManager의 모든 References 연결됨
- [ ] Console에 에러 없음

---

## 🎮 테스트

**Play 버튼 누르기**

정상 작동 확인:
- ✅ 화면 중앙에 마법진이 천천히 그려짐
- ✅ 타이머가 3초부터 감소
- ✅ 마우스 드래그 시 빨간 선 표시
- ✅ 마법진을 자르면 파티클 효과 + 화면 쉐이크
- ✅ 3개 자르면 "SUCCESS!" 메시지

---

## 💡 Inspector에서 조정 가능한 값들

### GameManager
- **Cast Time**: 마법 시전 시간 (짧을수록 어려움)
- **Cuts Needed To Win**: 성공 조건 (많을수록 어려움)
- **Spawn Position**: 마법진 위치

### MagicCircle Prefab
- **Circle Color**: 마법진 색상
- **Line Width**: 선 두께
- **Draw Duration**: 그려지는 속도

### SlashDetector
- **Slash Color**: 검기 색상
- **Slash Width**: 검기 두께
- **Trail Duration**: 잔상 시간

---

이제 값을 자유롭게 조정하며 테스트할 수 있습니다! 🎨
